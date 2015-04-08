using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace DeanZhou.Framework
{

    /// <summary>
    /// 通用扩展类库
    /// </summary>
    public static class CommonExtension
    {
        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static T GetValue<T>(this object sourceValue)
        {
            return sourceValue.GetValue(default(T));
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <typeparam name="T">返回数据的类型</typeparam>
        /// <param name="sourceValue">源数据</param>
        /// <param name="defaultValue"></param>
        /// <returns>源数据转换成指定类型后的值</returns>
        public static T GetValue<T>(this object sourceValue, T defaultValue)
        {
            if (sourceValue == null || string.IsNullOrEmpty(sourceValue.ToString()))
            {
                return defaultValue;
            }
            try
            {
                T t = (T)Convert.ChangeType(sourceValue, typeof(T));
                return t;
            }
            catch (Exception)
            {
                return (T)sourceValue;
            }
        }

        /// <summary>
        /// 数据类型转换
        /// </summary>
        /// <typeparam name="T">返回数据的类型</typeparam>
        /// <param name="sourceValue">源数据</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>源数据转换成指定类型后的值</returns>
        public static T TryGetValue<T>(this object sourceValue, T defaultValue)
        {
            try
            {
                return sourceValue.GetValue(defaultValue);
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static T ChangeType<T>(this object sourceValue)
        {
            return sourceValue.GetValue<T>();
        }

        /// <summary>
        /// 拆箱
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static T TryChangeType<T>(this object sourceValue, T defaultValue)
        {
            return sourceValue.TryGetValue(defaultValue);
        }

        /// <summary>
        /// 对象clone
        /// 慎用 大对象复制 导致内存溢出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static T CloneObj<T>(this T sourceValue)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memStream, sourceValue);
                memStream.Seek(0, SeekOrigin.Begin);
                object obj = binaryFormatter.Deserialize(memStream);
                return obj.ChangeType<T>();
            }
        }

        /// <summary>
        /// clone DataTable
        /// </summary>
        /// <param name="sourceTable"></param>
        /// <returns></returns>
        public static DataTable CloneTable(this DataTable sourceTable)
        {
            if (sourceTable == null) return null;
            return sourceTable.Copy();
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsNumber(this string source)
        {
            Regex reg = new Regex("^[0-9]+$");
            Match ma = reg.Match(source);
            return ma.Success;
        }

        /// <summary>
        /// 格式化日期字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ToFormatString(this DateTime dt, string format = "yyyy-MM-dd HH:mm:ss.fff")
        {
            return dt.ToString(format);
        }

        /// <summary>
        /// 根据DateTime计算是周几 1 2 3 4 5 6 7
        /// </summary>
        /// <param name="dtNow"></param>
        /// <returns></returns>
        public static int CaculateWeekDay(this DateTime dtNow)
        {
            int dayOfWeek = (int)dtNow.DayOfWeek;
            return dayOfWeek == 0 ? 7 : dayOfWeek;
        }

        /// <summary>
        /// 将字符串写入文本文件
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="filePath"></param>
        public static void MakeStrToTxtFile(this string strContent, string filePath = "c://test.txt")
        {
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                StreamWriter sw = new StreamWriter(fs);
                fs.SetLength(0);//首先把文件清空了。
                sw.Write(strContent);//写你的字符串。
                sw.Close();
            }
        }

        /// <summary>
        /// 根据DataTable中的数据生成实体类集合
        /// </summary>
        /// <typeparam name="T">实体层的类</typeparam>
        /// <param name="dt">表</param>
        /// <returns>集合</returns>
        public static List<T> GetEntityListByTable<T>(this DataTable dt) where T : new()
        {
            List<T> entityList = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T t = (T)Activator.CreateInstance(typeof(T));
                foreach (DataColumn dc in dt.Columns)
                {
                    PropertyInfo field = t.GetType().GetProperty(dc.ColumnName);

                    if (field == null)
                        continue;

                    if (null == dr[dc.ColumnName] || Convert.IsDBNull(dr[dc.ColumnName]))
                        field.SetValue(t, null, null);
                    else
                        field.SetValue(t, dr[dc.ColumnName], null);

                }
                entityList.Add(t);
            }

            return entityList.Count == 0 ? null : entityList;
        }

        /// <summary>
        /// 根据IDataReader中的数据生成实体类集合
        /// </summary>
        /// <typeparam name="T">实体层的类</typeparam>
        /// <param name="dr">dr</param>
        /// <returns>集合</returns>
        public static List<T> GetEntityList<T>(this IDataReader dr) where T : new()
        {
            List<T> entityList = new List<T>();

            int fieldCount = -1;

            while (dr.Read())
            {
                if (-1 == fieldCount)
                    fieldCount = dr.FieldCount;

                T t = (T)Activator.CreateInstance(typeof(T));

                for (int i = 0; i < fieldCount; i++)
                {
                    PropertyInfo field = t.GetType().GetProperty(dr.GetName(i), BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                    if (field == null)
                        continue;

                    if (null == dr[i] || Convert.IsDBNull(dr[i]))
                        field.SetValue(t, null, null);
                    else if (dr[i] is decimal)
                        field.SetValue(t, Convert.ToDecimal(dr[i]), null);
                    else if (dr[i] is double || dr[i] is float)
                        field.SetValue(t, Convert.ToDouble(dr[i]), null);
                    else
                        field.SetValue(t, dr[i], null);
                }
                entityList.Add(t);
            }

            dr.Close();

            if (entityList.Count == 0)
                return null;

            return entityList;
        }

        /// <summary>
        /// 反射输出对象的所有属性的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        public static string GetReflectPropsValue<T>(this T t, string space = "") where T : class
        {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo p in t.GetType().GetProperties())
            {
                if (p.GetValue(t) is IList)
                {
                    sb.Append(string.Format("{1}<b>{0}</b>:|", p.Name, space));
                    IList ls = p.GetValue(t) as IList;
                    if (ls != null && ls.Count > 0)
                    {
                        foreach (var l in ls)
                        {
                            sb.Append(l.GetReflectPropsValue(space + "        "));
                            sb.Append("|");
                        }
                        sb.Append("|");
                    }
                    else
                    {
                        sb.Append(string.Format("{0}Count:0|",
                            space + "        "));
                    }
                }
                else
                {
                    sb.Append(string.Format("{2}[{0}]:{1}|", p.Name, p.GetValue(t).ToString().Replace("|", " "), space));
                }
            }
            return sb.ToString().Trim('|');
        }

        /// <summary>
        /// 判断枚举是否包含子项
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sourceItem"></param>
        /// <param name="targetItem"></param>
        /// <returns></returns>
        public static bool HasItem<T>(this T sourceItem, T targetItem) where T : struct
        {
            return (sourceItem.ChangeType<int>() & targetItem.ChangeType<int>()) == targetItem.ChangeType<int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ConvertToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strJson"></param>
        /// <returns></returns>
        public static T ConvertToObject<T>(this string strJson)
        {
            return JsonConvert.DeserializeObject<T>(strJson);
        }

        /// <summary>
        /// 字节数组生成图片
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <returns>图片</returns>
        public static Image ByteArrayToImage(this byte[] bytes)
        {
            if (bytes == null)
            {
                return null;
            }
            try
            {
                MemoryStream ms = new MemoryStream(bytes);
                Image outputImg = Image.FromStream(ms);
                return outputImg;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
