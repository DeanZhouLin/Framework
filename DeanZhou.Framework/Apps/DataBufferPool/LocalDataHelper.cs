using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JFx.Utils;

namespace DeanZhou.Framework
{
    [Serializable]
    public class DOBase
    {
        public int ID { get; set; }
    }

    public class LocalDB<T> where T : DOBase
    {
        private static List<T> allSource;

        private LocalDB()
        {

        }

        private static void RegistLocalDB()
        {
            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db");
            if (!Directory.Exists(dbFilePath))
            {
                Directory.CreateDirectory(dbFilePath);
            }

            string dbFileName = (typeof(T).Namespace + typeof(T).Name).Replace(".", "_") + ".db";
            string fileName = dbFilePath + "/" + dbFileName;
            if (File.Exists(fileName))
            {
                try
                {
                    string allStr = File.ReadAllText(fileName);
                    allSource = SerializerHelper.JsonDeserialize<List<T>>(allStr);
                }
                catch (Exception)
                {
                    File.Delete(fileName);
                }
            }

            if (allSource == null)
            {
                allSource = new List<T>();
            }

            AutoClock.Regist(() =>
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
            }, 1);
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
            return allSource.CloneObj();
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
