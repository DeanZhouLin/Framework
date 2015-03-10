using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace JFx.Utils
{
    /// <summary>
    /// XML、Json序列化助手
    /// <para>依赖于：Newtonsoft.Json.dll</para>
    /// </summary>
    public static class SerializerHelper
    {
        #region XML
        /// <summary>
        /// 序列化成XML格式字符串
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <returns>序列化后的字符串</returns>
        public static string XmlSerialize(object obj)
        {
            if (obj == null)
                return null;

            XmlSerializer xs = new XmlSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(ms, System.Text.Encoding.UTF8);
            StreamReader sr = null;
            string str;
            try
            {
                xtw.Formatting = System.Xml.Formatting.Indented;
                xs.Serialize(xtw, obj);
                ms.Seek(0, SeekOrigin.Begin);
                sr = new StreamReader(ms);
                str = sr.ReadToEnd();

            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                }
                if (xtw != null)
                {
                    xtw.Close();
                }
                if (ms != null)
                {
                    ms.Close();
                }
            }
            return str;
        }

        /// <summary>
        /// 从XML格式字符串反序列化 
        /// </summary>
        /// <param name="xml">xml字符串</param>
        /// <param name="type">要生成的对象类型</param>
        /// <returns>反序列化后的对象</returns>
        public static object XmlDeserialize(string xml, Type type)
        {
            if (string.IsNullOrEmpty(xml))
                return null;
            XmlSerializer xs = new XmlSerializer(type);
            StringReader sr = new StringReader(xml);
            object obj = xs.Deserialize(sr);
            return obj;
        }
        #endregion

        #region Json
        /// <summary>
        /// 以Json方式序列化指定对象
        /// </summary>
        /// <param name="obj">待序列化的对象</param>
        /// <param name="datetimeFormat">解决DateTime序列化后为Date(1411797836360+0800)格式问题，若需要生成Date(1411797836360+0800)格式，请忽略此参数，格式：yyyy-MM-dd HH:mm:ss</param>
        /// <returns>序列化后的Json文本</returns>
        public static string JsonSerializer(object obj, string datetimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            IsoDateTimeConverter dtConverter = new IsoDateTimeConverter { DateTimeFormat = datetimeFormat };;
            if (!string.IsNullOrEmpty(datetimeFormat))
            {
                return JsonConvert.SerializeObject(obj, dtConverter);
            }
            return JsonConvert.SerializeObject(obj);
        }
        /// <summary>
        /// 从Json格式字符串反序列化
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">json字符串</param>
        /// <returns>序列化后的对象</returns>
        public static T JsonDeserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        #endregion
    }
}
