using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace DeanZhou.Framework
{
    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpHelper
    {
        //默认的编码
        private Encoding _encoding = Encoding.Default;

        //Post数据编码
        private Encoding _postencoding = Encoding.Default;

        //HttpWebRequest对象用来发起请求
        private HttpWebRequest _request;

        //获取影响流的数据对象
        private HttpWebResponse _response;

        /// <summary>
        /// 根据相传入的数据，得到相应页面数据
        /// </summary>
        /// <param name="objHttpItem">参数类对象</param>
        /// <returns>返回HttpResult类型</returns>
        public HttpResult GetHtml(HttpItem objHttpItem)
        {
            //返回参数
            var result = new HttpResult();
            try
            {
                //准备参数
                SetRequest(objHttpItem);
            }
            catch (Exception ex)
            {
                result = new HttpResult
                {
                    Cookie = string.Empty,
                    Header = null,
                    Html = ex.Message,
                    StatusDescription = "配置参数时出错：" + ex.Message
                };
                return result;
            }
            try
            {
                using (_response = (HttpWebResponse)_request.GetResponse())
                {
                    result.StatusCode = _response.StatusCode;
                    result.StatusDescription = _response.StatusDescription;
                    result.Header = _response.Headers;
                    if (_response.Cookies != null) result.CookieCollection = _response.Cookies;
                    if (_response.Headers["set-cookie"] != null) result.Cookie = _response.Headers["set-cookie"];
                    //GZIIP处理
                    MemoryStream stream =
                        GetMemoryStream(_response.ContentEncoding.Equals("gzip",
                            StringComparison.InvariantCultureIgnoreCase)
                            ? // ReSharper disable AssignNullToNotNullAttribute
                            new GZipStream(_response.GetResponseStream(), CompressionMode.Decompress)
                            : // ReSharper restore AssignNullToNotNullAttribute
                            _response.GetResponseStream());
                    //获取Byte
                    byte[] rawResponse = stream.ToArray();
                    stream.Close();
                    //是否返回Byte类型数据
                    if (objHttpItem.ResultType == ResultType.Byte) result.ResultByte = rawResponse;
                    //从这里开始我们要无视编码了
                    if (_encoding == null)
                    {
                        Match meta = Regex.Match(Encoding.Default.GetString(rawResponse),
                            "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                        string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
                        if (charter.Length > 2)
                            _encoding =
                                Encoding.GetEncoding(
                                    charter.Trim()
                                        .Replace("\"", "")
                                        .Replace("'", "")
                                        .Replace(";", "")
                                        .Replace("iso-8859-1", "gbk"));
                        else
                        {
                            _encoding = string.IsNullOrEmpty(_response.CharacterSet)
                                ? Encoding.UTF8
                                : Encoding.GetEncoding(_response.CharacterSet);
                        }
                    }
                    //得到返回的HTML
                    result.Html = _encoding.GetString(rawResponse);
                }
            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                _response = (HttpWebResponse)ex.Response;
                result.Html = ex.Message;
                if (_response != null)
                {
                    result.StatusCode = _response.StatusCode;
                    result.StatusDescription = _response.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                result.Html = ex.Message;
            }
            if (objHttpItem.IsToLower) result.Html = result.Html.ToLower();
            return result;
        }

        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse">流</param>
        private static MemoryStream GetMemoryStream(Stream streamResponse)
        {
            var stream = new MemoryStream();
            const int length = 256;
            var buffer = new Byte[length];
            int bytesRead = streamResponse.Read(buffer, 0, length);
            while (bytesRead > 0)
            {
                stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, length);
            }
            return stream;
        }

        /// <summary>
        /// 为请求准备参数
        /// </summary>
        ///<param name="objhttpItem">参数列表</param>
        private void SetRequest(HttpItem objhttpItem)
        {
            // 验证证书
            SetCer(objhttpItem);
            //设置Header参数
            if (objhttpItem.Header != null && objhttpItem.Header.Count > 0) foreach (string item in objhttpItem.Header.AllKeys)
                {
                    _request.Headers.Add(item, objhttpItem.Header[item]);
                }
            // 设置代理
            SetProxy(objhttpItem);
            if (objhttpItem.ProtocolVersion != null) _request.ProtocolVersion = objhttpItem.ProtocolVersion;
            _request.ServicePoint.Expect100Continue = objhttpItem.Expect100Continue;
            //请求方式Get或者Post
            _request.Method = objhttpItem.Method;
            _request.Timeout = objhttpItem.Timeout;
            _request.ReadWriteTimeout = objhttpItem.ReadWriteTimeout;
            //Accept
            _request.Accept = objhttpItem.Accept;
            //ContentType返回类型
            _request.ContentType = objhttpItem.ContentType;
            //UserAgent客户端的访问类型，包括浏览器版本和操作系统信息
            _request.UserAgent = objhttpItem.UserAgent;
            // 编码
            _encoding = objhttpItem.Encoding;
            //设置Cookie
            SetCookie(objhttpItem);
            //来源地址
            _request.Referer = objhttpItem.Referer;
            //是否执行跳转功能
            _request.AllowAutoRedirect = objhttpItem.Allowautoredirect;
            //设置Post数据
            SetPostData(objhttpItem);
            //设置最大连接
            if (objhttpItem.Connectionlimit > 0) _request.ServicePoint.ConnectionLimit = objhttpItem.Connectionlimit;
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="objhttpItem"></param>
        private void SetCer(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.CerPath))
            {
                //这一句一定要写在创建连接的前面。使用回调的方法进行证书验证。
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
                //初始化对像，并设置请求的URL地址
                _request = (HttpWebRequest)WebRequest.Create(objhttpItem.URL);
                SetCerList(objhttpItem);
                //将证书添加到请求里
                _request.ClientCertificates.Add(new X509Certificate(objhttpItem.CerPath));
            }
            else
            {
                //初始化对像，并设置请求的URL地址
                _request = (HttpWebRequest)WebRequest.Create(objhttpItem.URL);
                SetCerList(objhttpItem);
            }
        }

        /// <summary>
        /// 设置多个证书
        /// </summary>
        /// <param name="objhttpItem"></param>
        private void SetCerList(HttpItem objhttpItem)
        {
            if (objhttpItem.ClentCertificates == null || objhttpItem.ClentCertificates.Count <= 0) return;
            foreach (X509Certificate item in objhttpItem.ClentCertificates)
            {
                _request.ClientCertificates.Add(item);
            }
        }

        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetCookie(HttpItem objhttpItem)
        {
            if (!string.IsNullOrEmpty(objhttpItem.Cookie))
                //Cookie
                _request.Headers[HttpRequestHeader.Cookie] = objhttpItem.Cookie;
            //设置Cookie
            if (objhttpItem.CookieCollection == null) return;
            _request.CookieContainer = new CookieContainer();
            _request.CookieContainer.Add(objhttpItem.CookieCollection);
        }

        /// <summary>
        /// 设置Post数据
        /// </summary>
        /// <param name="objhttpItem">Http参数</param>
        private void SetPostData(HttpItem objhttpItem)
        {
            //验证在得到结果时是否有传入数据
            if (!_request.Method.Trim().ToLower().Contains("post")) return;
            if (objhttpItem.PostEncoding != null)
            {
                _postencoding = objhttpItem.PostEncoding;
            }
            byte[] buffer = null;
            //写入Byte类型
            if (objhttpItem.PostDataType == PostDataType.Byte && objhttpItem.PostdataByte != null && objhttpItem.PostdataByte.Length > 0)
            {
                //验证在得到结果时是否有传入数据
                buffer = objhttpItem.PostdataByte;
            }//写入文件
            else if (objhttpItem.PostDataType == PostDataType.FilePath && !string.IsNullOrEmpty(objhttpItem.Postdata))
            {
                var r = new StreamReader(objhttpItem.Postdata, _postencoding);
                buffer = _postencoding.GetBytes(r.ReadToEnd());
                r.Close();
            } //写入字符串
            else if (!string.IsNullOrEmpty(objhttpItem.Postdata))
            {
                buffer = _postencoding.GetBytes(objhttpItem.Postdata);
            }
            if (buffer != null)
            {
                _request.ContentLength = buffer.Length;
                _request.GetRequestStream().Write(buffer, 0, buffer.Length);
            }
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="objhttpItem">参数对象</param>
        private void SetProxy(HttpItem objhttpItem)
        {
            if (string.IsNullOrEmpty(objhttpItem.ProxyIp)) return;
            //设置代理服务器
            if (objhttpItem.ProxyIp.Contains(":"))
            {
                string[] plist = objhttpItem.ProxyIp.Split(':');
                var myProxy = new WebProxy(plist[0].Trim(), Convert.ToInt32(plist[1].Trim()))
                {
                    Credentials = new NetworkCredential(objhttpItem.ProxyUserName, objhttpItem.ProxyPwd)
                };
                //建议连接
                //给当前请求对象
                _request.Proxy = myProxy;
            }
            else
            {
                var myProxy = new WebProxy(objhttpItem.ProxyIp, false)
                {
                    Credentials = new NetworkCredential(objhttpItem.ProxyUserName, objhttpItem.ProxyPwd)
                };
                //建议连接
                //给当前请求对象
                _request.Proxy = myProxy;
            }
            //设置安全凭证
            _request.Credentials = CredentialCache.DefaultNetworkCredentials;
        }

        /// <summary>
        /// 回调验证证书问题
        /// </summary>
        /// <param name="sender">流对象</param>
        /// <param name="certificate">证书</param>
        /// <param name="chain">X509Chain</param>
        /// <param name="errors">SslPolicyErrors</param>
        /// <returns>bool</returns>
        private static bool CheckValidationResult(object sender, X509Certificate certificate,
            X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

    }
}
