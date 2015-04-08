﻿using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.IO.Compression;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace JFx.Utils
{
    /// <summary>
    /// HttpHelper
    /// <para>调用方需进行异常处理</para>
    /// </summary>
    public class HttpHelper
    {
        private static bool RemoteCertificateValidate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //用户https请求
            return true; //总是接受
        }
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="data">POST数据</param>
        /// <exception cref="System.InvalidOperationException">流正由上一个 System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)调用使用。- 或 -System.Net.HttpWebRequest.TransferEncoding 被设置为一个值，并且 System.Net.HttpWebRequest.SendChunked为 false。</exception>
        /// <exception cref="System.Net.ProtocolViolationException">System.Net.HttpWebRequest.Method 为 GET 或 HEAD，并且或者 System.Net.HttpWebRequest.ContentLength大于或等于零，或者 System.Net.HttpWebRequest.SendChunked 为 true。- 或 -System.Net.HttpWebRequest.KeepAlive为 true，System.Net.HttpWebRequest.AllowWriteStreamBuffering 为 false，System.Net.HttpWebRequest.ContentLength为 -1，System.Net.HttpWebRequest.SendChunked 为 false，System.Net.HttpWebRequest.Method为 POST 或 PUT。</exception>
        /// <exception cref="System.NotSupportedException">请求缓存验证程序指示对此请求的响应可从缓存中提供；但是，此请求包含要发送到服务器的数据。发送数据的请求不得使用缓存。如果您正在使用错误实现的自定义缓存验证程序，则会发生此异常。</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort() 以前被调用过。- 或 -请求的超时期限到期。- 或 -处理请求时发生错误。</exception>
        /// <returns>Response数据</returns>
        public static string SendPost(string url, string data)
        {
            return Send(url, "POST", data, null);
        }

        /// <summary>
        /// 发送GET请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <exception cref="System.InvalidOperationException">流正由上一个 System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)调用使用。- 或 -System.Net.HttpWebRequest.TransferEncoding 被设置为一个值，并且 System.Net.HttpWebRequest.SendChunked为 false。</exception>
        /// <exception cref="System.Net.ProtocolViolationException">System.Net.HttpWebRequest.Method 为 GET 或 HEAD，并且或者 System.Net.HttpWebRequest.ContentLength大于或等于零，或者 System.Net.HttpWebRequest.SendChunked 为 true。- 或 -System.Net.HttpWebRequest.KeepAlive为 true，System.Net.HttpWebRequest.AllowWriteStreamBuffering 为 false，System.Net.HttpWebRequest.ContentLength为 -1，System.Net.HttpWebRequest.SendChunked 为 false，System.Net.HttpWebRequest.Method为 POST 或 PUT。</exception>
        /// <exception cref="System.NotSupportedException">请求缓存验证程序指示对此请求的响应可从缓存中提供；但是，此请求包含要发送到服务器的数据。发送数据的请求不得使用缓存。如果您正在使用错误实现的自定义缓存验证程序，则会发生此异常。</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort() 以前被调用过。- 或 -请求的超时期限到期。- 或 -处理请求时发生错误。</exception>
        /// <returns>Response数据</returns>
        public static string SendGet(string url)
        {
            return Send(url, "GET", null, null);
        }
        /// <summary>
        /// 发送WEB请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="method">POST、GET</param>
        /// <param name="data">POST数据</param>
        /// <param name="config">配置信息</param>
        /// <exception cref="System.InvalidOperationException">流正由上一个 System.Net.HttpWebRequest.BeginGetResponse(System.AsyncCallback,System.Object)调用使用。- 或 -System.Net.HttpWebRequest.TransferEncoding 被设置为一个值，并且 System.Net.HttpWebRequest.SendChunked为 false。</exception>
        /// <exception cref="System.Net.ProtocolViolationException">System.Net.HttpWebRequest.Method 为 GET 或 HEAD，并且或者 System.Net.HttpWebRequest.ContentLength大于或等于零，或者 System.Net.HttpWebRequest.SendChunked 为 true。- 或 -System.Net.HttpWebRequest.KeepAlive为 true，System.Net.HttpWebRequest.AllowWriteStreamBuffering 为 false，System.Net.HttpWebRequest.ContentLength为 -1，System.Net.HttpWebRequest.SendChunked 为 false，System.Net.HttpWebRequest.Method为 POST 或 PUT。</exception>
        /// <exception cref="System.NotSupportedException">请求缓存验证程序指示对此请求的响应可从缓存中提供；但是，此请求包含要发送到服务器的数据。发送数据的请求不得使用缓存。如果您正在使用错误实现的自定义缓存验证程序，则会发生此异常。</exception>
        /// <exception cref="System.Net.WebException">System.Net.HttpWebRequest.Abort() 以前被调用过。- 或 -请求的超时期限到期。- 或 -处理请求时发生错误。</exception>
        /// <returns>Response数据</returns>
        public static string Send(string url, string method, string data, HttpConfig config)
        {
            if (config == null) config = new HttpConfig();
            string result;
            using (HttpWebResponse response = GetResponse(url, method, data, config))
            {
                Stream stream = response.GetResponseStream();

                if (!String.IsNullOrEmpty(response.ContentEncoding))
                {
                    if (response.ContentEncoding.Contains("gzip"))
                    {
                        stream = new GZipStream(stream, CompressionMode.Decompress);
                    }
                    else if (response.ContentEncoding.Contains("deflate"))
                    {
                        stream = new DeflateStream(stream, CompressionMode.Decompress);
                    }
                }

                byte[] bytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    int count;
                    byte[] buffer = new byte[4096];
                    while ((count = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, count);
                    }
                    bytes = ms.ToArray();
                }

                #region 检测流编码
                Encoding encoding;

                //检测响应头是否返回了编码类型,若返回了编码类型则使用返回的编码
                //注：有时响应头没有编码类型，CharacterSet经常设置为ISO-8859-1
                if (!string.IsNullOrEmpty(response.CharacterSet) && response.CharacterSet.ToUpper() != "ISO-8859-1")
                {
                    encoding = Encoding.GetEncoding(response.CharacterSet == "utf8" ? "utf-8" : response.CharacterSet);
                }
                else
                {
                    //若没有在响应头找到编码，则去html找meta头的charset
                    result = Encoding.Default.GetString(bytes);
                    //在返回的html里使用正则匹配页面编码
                    Match match = Regex.Match(result, @"<meta.*charset=""?([\w-]+)""?.*>", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        encoding = Encoding.GetEncoding(match.Groups[1].Value);
                    }
                    else
                    {
                        //若html里面也找不到编码，默认使用utf-8
                        encoding = Encoding.GetEncoding(config.CharacterSet);
                    }
                }
                #endregion

                result = encoding.GetString(bytes);
            }
            return result;
        }

        private static HttpWebResponse GetResponse(string url, string method, string data, HttpConfig config)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Referer = config.Referer;
            //有些页面不设置用户代理信息则会抓取不到内容
            request.UserAgent = config.UserAgent;
            request.Timeout = config.Timeout;
            request.Accept = config.Accept;
            request.Headers.Set("Accept-Encoding", config.AcceptEncoding);
            request.ContentType = config.ContentType;
            request.KeepAlive = config.KeepAlive;

            if (url.ToLower().StartsWith("https"))
            {
                //这里加入解决生产环境访问https的问题--Could not establish trust relationship for the SSL/TLS secure channel
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(RemoteCertificateValidate);
            }


            if (method.ToUpper() == "POST")
            {
                if (!string.IsNullOrEmpty(data))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(data);

                    if (config.GZipCompress)
                    {
                        Console.WriteLine("压缩前:" + bytes.Length);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            using (GZipStream gZipStream = new GZipStream(stream, CompressionMode.Compress))
                            {
                                gZipStream.Write(bytes, 0, bytes.Length);
                            }
                            bytes = stream.ToArray();
                        }
                        Console.WriteLine("压缩后:" + bytes.Length);
                    }

                    request.ContentLength = bytes.Length;
                    request.GetRequestStream().Write(bytes, 0, bytes.Length);
                }
                else
                {
                    request.ContentLength = 0;
                }
            }

            return (HttpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// 解析URL(可以正确识别UTF-8和GB2312编码)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlDecode(string url)
        {
            //正则1：^(?:[\x00-\x7f]|[\xe0-\xef][\x80-\xbf]{2})+$
            //正则2：^(?:[\x00-\x7f]|[\xfc-\xff][\x80-\xbf]{5}|[\xf8-\xfb][\x80-\xbf]{4}|[\xf0-\xf7][\x80-\xbf]{3}|[\xe0-\xef][\x80-\xbf]{2}|[\xc0-\xdf][\x80-\xbf])+$
            //如果不考虑哪些什么拉丁文啊，希腊文啊。。。乱七八糟的外文，用短的正则，即正则1
            //如果考虑哪些什么拉丁文啊，希腊文啊。。。乱七八糟的外文，用长的正则，即正则2
            //本方法使用的正则1
            if (Regex.IsMatch(HttpUtility.UrlDecode(url, Encoding.GetEncoding("iso-8859-1")), @"^(?:[\x00-\x7f]|[\xe0-\xef][\x80-\xbf]{2})+$"))
            {
                return HttpUtility.UrlDecode(url, Encoding.GetEncoding("UTF-8"));
            }
            else
            {
                return HttpUtility.UrlDecode(url, Encoding.GetEncoding("GB2312"));
            }
        }
        /// <summary>
        /// 对 URL 字符串进行编码。
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string UrlEncode(string url)
        {
            return HttpUtility.UrlEncode(url);
        }

        /// <summary>
        /// 解析URL，返回查询字符串集合(已经正确识别UTF-8和GB2312编码)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static NameValueCollection ParseQuery(string url)
        {
            NameValueCollection query = new NameValueCollection();
            if (!string.IsNullOrEmpty(url))
            {
                if (url.Contains("?"))
                {
                    query = HttpUtility.ParseQueryString(UrlDecode(url.Substring(url.IndexOf("?"))));
                }
                else
                {
                    query = HttpUtility.ParseQueryString(UrlDecode(url));
                }
            }
            return query;
        }

        /// <summary>
        /// 解析URL返回域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string PraseDomain(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            try
            {
                Uri uri = new Uri(url);
                return uri.Host;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取客户端IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetClientIP()
        {
            string ip = string.Empty;
            if (System.Web.HttpContext.Current != null)
            {
                if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(',')[0];
                else
                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (string.IsNullOrEmpty(ip)) ip = System.Web.HttpContext.Current.Request.UserHostAddress;
            }
            return ip;
        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIP()
        {
            System.Net.IPHostEntry ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            return ips.AddressList.First(item => item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
        }
    }

    public class HttpConfig
    {
        public string Referer { get; set; }

        /// <summary>
        /// 默认(text/html)
        /// </summary>
        public string ContentType { get; set; }

        public string Accept { get; set; }

        public string AcceptEncoding { get; set; }

        /// <summary>
        /// 超时时间(毫秒)默认100000
        /// </summary>
        public int Timeout { get; set; }

        public string UserAgent { get; set; }

        /// <summary>
        /// POST请求时，数据是否进行gzip压缩
        /// </summary>
        public bool GZipCompress { get; set; }

        public bool KeepAlive { get; set; }

        public string CharacterSet { get; set; }

        public HttpConfig()
        {
            this.Timeout = 100000;
            this.ContentType = "text/html; charset=" + Encoding.UTF8.WebName;
            this.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/33.0.1750.117 Safari/537.36";
            this.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            this.AcceptEncoding = "gzip,deflate";
            this.GZipCompress = false;
            this.KeepAlive = true;
            this.CharacterSet = "UTF-8";
        }
    }
}