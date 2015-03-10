using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace JFx.Utils
{
    /// <summary>
    /// 工具类
    /// </summary>
    public class Utility
    {
        #region 执行指定Action并捕捉异常
        /// <summary>
        /// 执行指定Action并捕捉异常
        /// </summary>
        /// <param name="func">需要执行的Func</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns>action返回值，如果报错则返回defaultValue</returns>
        public static T InvokeFuncWithCatch<T>(Func<T> func, T defaultValue = default(T))
        {
            try
            {
                return func();
            }
            catch
            {
                return defaultValue;
            }
        }
        #endregion

        #region 获取请求IP地址
        /// <summary>
        /// 获取请求IP地址，默认返回：string.Empty
        /// </summary>
        /// <returns>默认返回：string.Empty</returns>
        public static string GetClientIp()
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
            if (ip.Length > 15)
                isErr = true;
            else
            {
                temp = ip.Split('.');
                if (temp.Length == 4)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 3) isErr = true;
                    }
                }
                else
                    isErr = true;
            }

            if (isErr)
                return string.Empty;
            else
                return ip;
        }
        #endregion

        #region 获取本机IP地址，非客户端IP
        /// <summary>
        /// 获取本机IP地址，非客户端IP，默认返回：string.Empty
        /// </summary>
        /// <returns>默认返回：string.Empty</returns>
        public static string GetLocalIPAddress()
        {
            var ipAddress = Dns.GetHostEntry(Dns.GetHostName())
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            if (ipAddress != null)
            {
                return ipAddress.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region 得到字符串长度，一个汉字长度为2
        /// <summary>
        /// 获取字符串长度，一个汉字长度为2
        /// </summary>
        /// <param name="inputString">参数字符串</param>
        /// <returns>字符串长度</returns>
        public static int StrLength(string inputString)
        {
            System.Text.ASCIIEncoding ascii = new System.Text.ASCIIEncoding();
            int tempLen = 0;
            byte[] s = ascii.GetBytes(inputString);
            for (int i = 0; i < s.Length; i++)
            {
                if ((int)s[i] == 63)
                    tempLen += 2;
                else
                    tempLen += 1;
            }
            return tempLen;
        }
        #endregion

        #region xupearl 2015-1-23 add 
        #region 数据判断
        /// <summary>
        /// 判断对象是否为正确的日期值。
        /// </summary>
        /// <param name="obj">对象。</param>
        /// <returns>Boolean。</returns>
        public static bool IsDateTime(object obj)
        {
            try
            {
                if (obj == null) { return false; }
                DateTime dt = DateTime.Parse(obj.ToString());
                if (dt > DateTime.MinValue && DateTime.MaxValue > dt)
                    return true;
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region 转换用户输入

        /// <summary>
        /// 将用户输入的字符串转换为可换行、替换Html编码、无危害数据库特殊字符、去掉首尾空白、的安全方便代码。
        /// </summary>
        /// <param name="inputString">用户输入字符串</param>
        public static string ConvertStr(string inputString)
        {
            string retVal = inputString;
            //retVal=retVal.Replace("&","&amp;"); 
            retVal = retVal.Replace("\"", "&quot;");
            retVal = retVal.Replace("<", "&lt;");
            retVal = retVal.Replace(">", "&gt;");
            retVal = retVal.Replace(" ", "&nbsp;");
            retVal = retVal.Replace("  ", "&nbsp;&nbsp;");
            retVal = retVal.Replace("\t", "&nbsp;&nbsp;");
            retVal = retVal.Replace("\r", "<br>");
            return retVal;
        }

        /// <summary>
        /// 将用户输入的字符串转换为可换行、替换Html编码、无危害数据库特殊字符、去掉首尾空白、的安全方便代码。替换掉url,/url等字符
        /// </summary>
        /// <param name="inputString">用户输入字符串</param>
        /// <returns></returns>
        public static string InputText(string inputString)
        {
            string retVal = inputString;
            retVal = ConvertStr(retVal);
            retVal = retVal.Replace("[url]", "");
            retVal = retVal.Replace("[/url]", "");
            return retVal;
        }


        /// <summary>
        /// 将html代码显示在网页上
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string OutputText(string inputString)
        {
            string retVal = System.Web.HttpUtility.HtmlDecode(inputString);
            retVal = retVal.Replace("<br>", "");
            retVal = retVal.Replace("&amp", "&;");
            retVal = retVal.Replace("&quot;", "\"");
            retVal = retVal.Replace("&lt;", "<");
            retVal = retVal.Replace("&gt;", ">");
            retVal = retVal.Replace("&nbsp;", " ");
            retVal = retVal.Replace("&nbsp;&nbsp;", "  ");
            return retVal;
        }

      
        #endregion

        #region   字符串长度区分中英文截取
        /// <summary>   
        /// 截取文本，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="str">待截取的字符串</param>
        /// <param name="length">需计算长度的字符串</param>
        /// <returns>string</returns>
        public static string GetSubString(string str, int length)
        {
            string temp = str;
            int j = 0;
            int k = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j > length)
                {
                    return temp.Substring(0, k) + "...";
                }
            }
            return temp;
        }
        #endregion

        /// <summary>
        /// 判断是否HTTPS
        /// </summary>
        /// <returns></returns>
        public static bool IsHttps()
        {
            bool isHttps = false;
            string serverName = HttpContext.Current.Request.ServerVariables["SERVER_NAME"].ToLower();

            if (!string.IsNullOrEmpty(serverName) && (serverName.IndexOf("https://") >= 0))
            {
                isHttps = true;
            }
            return isHttps;
        }


        /// <summary>
        /// 转换为星期信息，周一－周日依次为1－7
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ConvertToWeek<T>(T source)
        {
            DateTime? date = ConvertDateTimeOrNull(source);
            int result = 0;

            if (date.HasValue)
            {
                result = (int)date.Value.DayOfWeek;
                if (result == 0)
                {
                    result = 7;
                }
            }
            return result;
        }

        /// <summary>
        /// 转换为Nullable<DateTime>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static DateTime? ConvertDateTimeOrNull<T>(T source)
        {
            DateTime result;

            if (source == null)
            {
                return null;
            }
            else if (DateTime.TryParse(source.ToString(), out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        ///  转换DayOfWeek为int数值，周一－周日依次为1－7
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static int ConvertToWeek(DayOfWeek week)
        {
            int result = (int)week;

            if (result == 0)
            {
                result = 7;
            }
            return result;
        }
        /// <summary>
        /// 不为空时，四舍五入保留两位小数
        /// 为空时，返回空字符
        /// </summary>
        public static String ConvertToRound<T>(T source)
        {
            if (source != null)
            {
                if (!String.Empty.Equals(source.ToString()))
                {
                    decimal result = Math.Round((Convert.ToDecimal(source)), 2, MidpointRounding.AwayFromZero);
                    return result.ToString();
                }
                else { return source.ToString(); }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 转换百分数
        /// 如：15=>15%
        /// 为Null或空时，返回空字符
        /// </summary>
        public static String ConvertToPercentString<T>(T source)
        {
            if (source != null)
            {
                if (!String.Empty.Equals(source.ToString()))
                {
                    decimal result = Convert.ToDecimal(source);

                    if (result == 0)
                    {
                        return "0%";
                    }
                    else
                    {
                        return string.Format("{0:#%}", result / 100);
                    }
                }
                else { return source.ToString(); }
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 去除字符串中空格
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string ConvertToTrimString<T>(T source)
        {
            string result = source.ToString();

            if (!String.IsNullOrEmpty(result))
            {
                result = result.Trim();
            }
            return result;
        }
        #endregion
    }
}
