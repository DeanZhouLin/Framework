using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore
{
    /// <summary>
    /// 通用扩展方法
    /// </summary>
    public static class CommonExtension
    {
        /// <summary>
        /// javascript中GetTime()在c#中实现
        /// </summary>
        /// <returns></returns>
        public static double GetTime()
        {
            DateTime minValue = new DateTime(1970, 1, 1);
            DateTime nowValue = DateTime.Now;
            double value = (nowValue - minValue).TotalMilliseconds;
            return Math.Floor(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string NormalU2C(this string input)
        {
            string str = "";
            char[] chArray = input.ToCharArray();
            for (int i = 0; i < chArray.Length; i++)
            {
                char ch = chArray[i];
                if (ch.Equals('\\'))
                {
                    i++;
                    i++;
                    char[] chArray2 = new char[4];
                    var index = 0;
                    while ((index < 4) && (i < chArray.Length))
                    {
                        chArray2[index] = chArray[i];
                        index++;
                        i++;
                    }
                    if (index == 4)
                    {
                        try
                        {
                            str = str + UnicodeCode2Str(chArray2);
                        }
                        catch (Exception)
                        {
                            str = str + @"\u";
                            for (int j = 0; j < index; j++)
                            {
                                str = str + chArray2[j];
                            }
                        }
                        i--;
                    }
                    else
                    {
                        str = str + @"\u";
                        for (int k = 0; k < index; k++)
                        {
                            str = str + chArray2[k];
                        }
                    }
                }
                else
                {
                    str = str + ch;
                }
            }
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="u4"></param>
        /// <returns></returns>
        private static string UnicodeCode2Str(this IList<char> u4)
        {
            if (u4.Count < 4)
            {
                throw new Exception("It's not a unicode code array");
            }
            const string str = "0123456789ABCDEF";
            char ch = char.ToUpper(u4[0]);
            char ch2 = char.ToUpper(u4[1]);
            char ch3 = char.ToUpper(u4[2]);
            char ch4 = char.ToUpper(u4[3]);
            int index = str.IndexOf(ch);
            int num2 = str.IndexOf(ch2);
            int num3 = str.IndexOf(ch3);
            int num4 = str.IndexOf(ch4);
            if (((index == -1) || (num2 == -1)) || ((num3 == -1) || (num4 == -1)))
            {
                throw new Exception("It's not a unicode code array");
            }
            byte num5 = (byte)(((index * 0x10) + num2) & 0xff);
            byte num6 = (byte)(((num3 * 0x10) + num4) & 0xff);
            byte[] bytes = { num5, num6 };
            return Encoding.BigEndianUnicode.GetString(bytes);
        }

        /// <summary>
        /// 分析提交结果
        /// </summary>
        /// <param name="html"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CommonAnalyse(this string html, string type)
        {
            if (string.IsNullOrEmpty(html))
            {
                return string.Format("网络错误 {0}", type);
            }
            try
            {
                dynamic result = DynamicJson.Parse(html);
                string ok = result.ok;
                if (ok == "1")
                    return "";
                if (ok == "-100")
                    return "登录已失效";
                if (ok == "-9+225")
                    return "账号锁定";
                string msg = result.IsDefined("msg") ? result.msg : "空";
                if (type == "复制信息" && !string.IsNullOrEmpty(msg) && msg.Contains("nick is not avaliable"))
                {
                    return "昵称已存在";
                }
                return string.Format("{0}失败，错误代码{1}错误信息{2}", type, ok, msg);
            }
            catch (Exception)
            {
                return string.Format("分析{0}结果失败\r\n{1}", type, html);
            }
        }

        /// <summary>
        /// 分析新浪给的时间
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime AnalyseTime(this string dateStr)
        {
            if (dateStr.Contains("分钟前"))
            {
                return DateTime.Now.AddMinutes(-int.Parse(dateStr.Replace("分钟前", "")));
            }
            if (dateStr.Contains("今天 "))
            {
                dateStr = dateStr.Replace("今天", "");
                dateStr = DateTime.Now.ToString("yyyy-MM-dd") + dateStr;
            }
            if (dateStr.Length == 11)
            {
                dateStr = DateTime.Now.Year + "-" + dateStr;
            }
            return DateTime.Parse(dateStr);
        }

        /// <summary>
        /// 将9位mid转换为16位数字id
        /// 不满足9位的无法转换
        /// </summary>
        /// <param name="mid">9位mid</param>
        /// <returns>16位数字id</returns>
        public static string MidToId(this string mid)
        {
            if (string.IsNullOrEmpty(mid) || mid.Length != 9)
            {
                return "";
            }

            var first = Converter.Str2Decimal(mid.Substring(0, 1));
            var second = Converter.Str2Decimal(mid.Substring(1, 4));
            var three = Converter.Str2Decimal(mid.Substring(5, 4));
            //由于计算的数字结果可能不足7位，需要在前面补足0，保证id长度为16位
            return string.Format("{0}{1}{2}", first, second.ToString(CultureInfo.InvariantCulture).PadLeft(7, '0'),
                three.ToString(CultureInfo.InvariantCulture).PadLeft(7, '0'));
        }

        /// <summary>
        /// 将16位数字id转换为9位mid
        /// 不满足16位的无法转换
        /// </summary>
        /// <param name="id">16位数字id</param>
        /// <returns>9位mid</returns>
        public static string IdToMid(this string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 16)
            {
                return "";
            }

            var first = Converter.Decimal2Str(decimal.Parse(id.Substring(0, 2)));
            var second = Converter.Decimal2Str(decimal.Parse(id.Substring(2, 7))).PadLeft(4, '0');
            var three = Converter.Decimal2Str(decimal.Parse(id.Substring(9, 7))).PadLeft(4, '0');
            //由于计算的数字结果可能不足4位，需要在前面补足0，保证mid长度为9位
            return string.Format("{0}{1}{2}", first, second, three);
        }

        /// <summary>
        /// 检查文本长度
        /// </summary>
        /// <param name="text"></param>
        /// <param name="len"></param>
        public static void CheckLength(this string text, int len)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (Encoding.GetEncoding("GBK").GetByteCount(text) * 2 > len)
            {
                throw new Exception("文本内容超出" + len + "字节");
            }
        }

        /// <summary>
        /// 根据picPidOrPaths获取pid
        /// </summary>
        /// <param name="picPidOrPaths">picPidOrPaths</param>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="joinLetter">多个pid之间的连接符号</param>
        /// <returns>pid1 pid2 pid3</returns>
        /// <exception cref="Exception"></exception>
        public static string GetPicsPid(this string[] picPidOrPaths, IWeiboLogin webLogin, string joinLetter = " ")
        {
            if (picPidOrPaths == null || !picPidOrPaths.Any())
            {
                return "";
            }

            List<string> pids = new List<string>();
            List<string> paths = new List<string>();
            foreach (string picPidOrPath in picPidOrPaths.Where(c => !string.IsNullOrEmpty(c)))
            {
                //path
                if (picPidOrPath.Contains("."))
                {
                    if (picPidOrPath.Substring(picPidOrPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "jpg" &&
                        picPidOrPath.Substring(picPidOrPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "png" &&
                        picPidOrPath.Substring(picPidOrPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "gif")
                    {
                        throw new Exception(string.Format("图片文件扩展名错误（{0}）", picPidOrPath));
                    }
                    paths.Add(picPidOrPath);
                }
                //pid
                else
                {
                    pids.Add(picPidOrPath);
                }
            }

            //path转pid
            if (paths.Any())
            {
                pids.AddRange(webLogin.CurrPlatformType.GetHttpWork().AddPicture(webLogin, paths.ToArray()));
            }

            return string.Join(joinLetter, pids);
        }
    }

    /// <summary>
    /// 转换器
    /// </summary>
    public static class Converter
    {
        private const string Keys = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"; //编码,可加一些字符也可以实现72,96等任意进制转换，但是有符号数据不直观，会影响阅读。
        private static readonly int Exponent = Keys.Length;//幂数

        /// <summary>
        /// ulong value type to 62 string
        /// </summary>
        /// <param name="value">The max value can not more decimal.MaxValue</param>
        /// <returns>Return a specified 62 encode string</returns>
        public static string Decimal2Str(decimal value)//17223472558080896352ul
        {
            string result = string.Empty;
            do
            {
                decimal index = value % Exponent;
                result = Keys[(int)index] + result;
                value = (value - index) / Exponent;
            }
            while (value > 0);

            return result;
        }

        /// <summary>
        /// 62 encode string to decimal
        /// </summary>
        /// <param name="value">62 encode string</param>
        /// <returns>Return a specified decimal number that decode by 62 string</returns>
        public static decimal Str2Decimal(string value)//bUI6zOLZTrj
        {
            decimal result = 0;
            for (int i = 0; i < value.Length; i++)
            {
                int x = value.Length - i - 1;
                result += Keys.IndexOf(value[i]) * Pow(Exponent, x);// Math.Pow(exponent, x);
            }
            return result;
        }

        /// <summary>
        /// 一个数据的N次方
        /// </summary>
        /// <param name="baseNo"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static decimal Pow(decimal baseNo, decimal x)
        {
            decimal value = 1;////1 will be the result for any number's power 0.任何数的0次方，结果都等于1
            while (x > 0)
            {
                value = value * baseNo;
                x--;
            }
            return value;
        }
    }
}
