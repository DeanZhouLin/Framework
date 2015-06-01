using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JFx.Utils;

namespace DeanZhou.Framework
{
    [Serializable]
    public class SysTimeRecord : DOBase
    {
        public string SysName { get; set; }

        public DateTime RecordTime { get; set; }

        private static readonly object _lockObj = new object();

        public static DateTime Get(string sysName)
        {
            lock (_lockObj)
            {
                List<SysTimeRecord> ls = LocalDB<SysTimeRecord>.Select();
                if (ls.Exists(c => c.SysName == sysName))
                {
                    return ls.First(c => c.SysName == sysName).RecordTime;
                }
                return DateTime.MinValue;
            }
        }

        public static void Set(string sysName, DateTime setDt)
        {
            lock (_lockObj)
            {
                List<SysTimeRecord> ls = LocalDB<SysTimeRecord>.Select();
                if (ls.Exists(c => c.SysName == sysName))
                {
                    ls.First(c => c.SysName == sysName).RecordTime = setDt;
                    return;
                }
                SysTimeRecord tem = new SysTimeRecord { SysName = sysName, RecordTime = setDt };
                tem.Insert();
            }
        }
    }

    [Serializable]
    public class DOBase
    {
        public int ID { get; set; }
    }

    public class LocalDB<T> where T : DOBase
    {
        private static List<T> allSource;

        LocalDB()
        {
        }

        private static void RegistLocalDB()
        {
            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db");
            if (!Directory.Exists(dbFilePath))
            {
                Directory.CreateDirectory(dbFilePath);
            }

            string dbFileName = (typeof(T).Namespace + "_" + typeof(T).Name).Replace(".", "_") + ".db";
            string fileName = dbFilePath + "/" + dbFileName;
            if (File.Exists(fileName))
            {
                try
                {
                    string allStr = File.ReadAllText(fileName);
                    allSource = SerializerHelper.JsonDeserialize<List<T>>(allStr);
                }
                catch (Exception ex)
                {
                    LogHelper.CustomInfoEnabled = true;
                    LogHelper.CustomInfo(ex, "RegistLocalDB_JsonDeserialize_ERR");
                    File.Delete(fileName);
                }
            }

            if (allSource == null)
            {
                allSource = new List<T>();
            }

            AutoClock.Regist(() =>
            {
                try
                {
                    FileInfo dbFile = new FileInfo(fileName);
                    FileStream fs = dbFile.Open(FileMode.Create, FileAccess.Write);
                    char[] r = SerializerHelper.JsonSerializer(allSource).ToCharArray();
                    Encoder e = Encoding.UTF8.GetEncoder();
                    byte[] byData = new byte[r.Length];
                    e.GetBytes(r, 0, r.Length, byData, 0, true);
                    fs.Write(byData, 0, byData.Length);
                    fs.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    LogHelper.CustomInfoEnabled = true;
                    LogHelper.CustomInfo(ex, "RegistLocalDB_AutoClock_ERR");
                }
            }, 3);
        }

        public static void Insert(T p)
        {
            if (allSource == null)
            {
                RegistLocalDB();
            }

            if (allSource != null && allSource.Any())
            {
                p.ID = allSource.Max(c => c.ID) + 1;
            }
            else
            {
                p.ID = 1;
            }

            if (allSource != null) allSource.Add(p);
        }

        public static void Delete(int id)
        {
            if (allSource == null)
            {
                RegistLocalDB();
            }
            if (allSource == null)
            {
                return;
            }
            for (int i = allSource.Count - 1; i >= 0; i--)
            {
                if (allSource[i].ID == id)
                {
                    allSource.RemoveAt(i);
                }
            }
        }

        public static void Update(T p)
        {
            Delete(p.ID);
            Insert(p);
        }

        public static List<T> Select()
        {
            if (allSource == null)
            {
                RegistLocalDB();
            }
            return allSource;
        }
    }

    public static class LocalDB
    {
        public static void Insert<T>(this T p) where T : DOBase
        {
            LocalDB<T>.Insert(p);
        }

        public static void Delete<T>(this T p) where T : DOBase
        {
            LocalDB<T>.Delete(p.ID);
        }

        public static void Update<T>(this T p) where T : DOBase
        {
            LocalDB<T>.Update(p);
        }

        public static List<T> Select<T>() where T : DOBase
        {
            return LocalDB<T>.Select();
        }
    }
}
