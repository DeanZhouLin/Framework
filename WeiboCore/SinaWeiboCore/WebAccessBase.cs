using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SinaWeiboCore
{
    /// <summary>
    /// 使用说明：
    /// 一个该类的实体只能使用一个账号访问一个网站,使用不同账号登陆同一网站或打开不同网站必须用不同的实例,以避免COOKIE出现混乱或错误
    /// 公共方法的Url参数须包括"http://"或"https://"
    /// 创建类实例后可设置： web访问无响应超时时间-TimeOut(默认5秒)和 网页编码方式-Encode(默认UTF8)
    /// 可在POST/GET前设置Reffer
    /// 若POST/GET返回值为null则函数执行中有异常，异常详情可通过Error查看
    /// </summary>
    public class WebAccessBase
    {
        /// <summary>
        /// HTTP头的Reffer值，即该次访问是由哪个网页转向来的

        /// </summary>
        protected Uri reffer;

        /// <summary>
        /// 增量保存每次POST或GET获取到的新COOKIE
        /// </summary>
        protected CookieContainer Cookies = new CookieContainer();

        /// <summary>
        /// POST GET访问无响应超时时间（ms）

        /// </summary>
        protected int timeOut;

        /// <summary>
        /// 网页的编码方式

        /// </summary>
        protected Encoding encoding;

        /// <summary>
        /// 异常情况
        /// </summary>
        protected Exception error;

        /// <summary>
        /// 代理网关对象
        /// </summary>
        protected WebProxy proxy;

        /// <summary>
        /// 
        /// </summary>
        public string UserAgent;

        /// <summary>
        /// 
        /// </summary>
        public WebAccessBase()
        {
            timeOut = 10000; //默认超时10秒

            encoding = Encoding.UTF8; //默认网页编码UTF8 
            proxy = new WebProxy { Address = null }; //初始化代理网关对象

            UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAgent"></param>
        public WebAccessBase(string userAgent)
        {
            timeOut = 10000; //默认超时10秒
            encoding = Encoding.UTF8; //默认网页编码UTF8 
            proxy = new WebProxy(); //初始化代理网关对象

            proxy.Address = null;
            UserAgent = userAgent;
        }

        /// <summary>
        /// 设置或读取POST GET访问无响应超时时间（ms）

        /// </summary>
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }

        /// <summary>
        /// 设置或读取网页的编码方式
        /// </summary>
        public Encoding Encode
        {
            get { return encoding; }
            set { encoding = value; }
        }

        /// <summary>
        /// 获取异常详情
        /// </summary>
        public Exception Error
        {
            get { return error; }
        }

        /// <summary>
        /// 设置或读取HTTP头的Reffer值，即该次访问是由哪个网页转向来的

        /// </summary>
        public Uri Reffer
        {
            get { return reffer; }
            set { reffer = value; }
        }

        /// <summary>
        /// 获取或设置Cookie
        /// </summary>
        public CookieContainer Cookie
        {
            get { return Cookies; }
            set { Cookies = value; }
        }

        /// <summary>
        /// 设置HTTPS响应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        protected bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            //直接确认，否则打不开HTTPS
            return true;
        }

        /// <summary>
        /// HTTP/HTTPS的GET方法获取HttpWebResponse
        /// </summary>
        /// <param name="Url">要GET数据的网址</param>
        /// <param name="Response"></param>
        /// <param name="rqst"></param>
        /// <param name="level"></param>
        /// <returns>HttpWebResponse</returns>
        protected void GetResponse(String Url, ref HttpWebResponse Response, ref HttpWebRequest rqst, int level = 0)
        {
            //如果使用代理https变为http
            //if (Url.ToLower().Contains("https://") && proxy.Address != null)
            //    Url = "http" + Url.Substring(5);
            Uri uri = new Uri(Url);
            //设置HTTPS方式
            if (Url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //构造HTTP头

            rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.KeepAlive = true;
            rqst.AllowAutoRedirect = false;
            rqst.ServicePoint.Expect100Continue = false;
            rqst.MaximumAutomaticRedirections = 3;
            rqst.UserAgent = UserAgent;
            if (reffer != null)
            {
                try
                {
                    rqst.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            rqst.Timeout = timeOut;
            rqst.CookieContainer = Cookies;
            if (proxy.Address != null)
            {
                rqst.ProtocolVersion = HttpVersion.Version10;
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.Proxy = proxy;
            }
            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;
            //访问URL
            Response = (HttpWebResponse)rqst.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            //防止无限循环跳转
            if (level < 10)
            {
                //自动重定向网页

                if (Response.Headers["Location"] != null)
                {
                    reffer = null;
                    Response.Close();
                    rqst.Abort();
                    String location = Response.Headers["Location"];
                    if (!location.Contains("://"))
                        location = uri.Scheme + "://" + uri.Authority + location;
                    GetResponse(location, ref Response, ref rqst, ++level);
                }
                else if (Response.ResponseUri != uri)
                {
                    Response.Close();
                    rqst.Abort();
                    GetResponse(Response.ResponseUri.AbsoluteUri, ref Response, ref rqst, ++level);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Response"></param>
        /// <param name="rqst"></param>
        /// <param name="level"></param>
        protected void GetResponse1(String Url, ref HttpWebResponse Response, ref HttpWebRequest rqst, int level = 0)
        {
            //如果使用代理https变为http
            //if (Url.ToLower().Contains("https://") && proxy.Address != null)
            //    Url = "http" + Url.Substring(5);
            Uri uri = new Uri(Url);
            //设置HTTPS方式
            if (Url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //构造HTTP头
            rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.KeepAlive = true;
            rqst.AllowAutoRedirect = true;
            rqst.ServicePoint.Expect100Continue = false;
            rqst.MaximumAutomaticRedirections = 30;
            rqst.UserAgent = UserAgent;
            rqst.Headers.Add("Accept-Encoding: gzip,deflate");
            if (reffer != null)
            {
                //try
                //{
                rqst.Referer = reffer.AbsoluteUri;
                //}
                //catch { }
            }
            rqst.Timeout = timeOut;
            rqst.CookieContainer = Cookies;
            if (proxy.Address != null)
            {
                rqst.ProtocolVersion = HttpVersion.Version10;
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.Proxy = proxy;
            }
            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;
            //访问URL
            //Response = (HttpWebResponse)rqst.GetResponse();
            using (ManualResetEventSlim mres = new ManualResetEventSlim(false))
            {
                HttpWebResponse resp = null;
                IAsyncResult ir = rqst.BeginGetResponse(r =>
                {
                    try
                    {
                        HttpWebRequest requ = r.AsyncState as HttpWebRequest;
                        resp = (HttpWebResponse)requ.EndGetResponse(r);
                    }
                    catch (Exception e)
                    {
                        resp = null;
                        error = e;
                    }
                    finally
                    {
                        mres.Set();
                    }
                }, rqst);
                mres.Wait();
                Response = resp;
            }
            //int k = 0;
            //int lsf = k;
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            //防止无限循环跳转
            //if (level < 5)
            //{
            //    //自动重定向网页
            //    if (Response.Headers["Location"] != null)
            //    {
            //        Response.Close();
            //        rqst.Abort();
            //        String location = Response.Headers["Location"];
            //        if (!location.Contains("://"))
            //            location = uri.Scheme + "://" + uri.Authority + location;
            //        GetResponse(location, ref Response, ref rqst, ++level);
            //    }
            //    else if (Response.ResponseUri != uri)
            //    {
            //        Response.Close();
            //        rqst.Abort();
            //        GetResponse(Response.ResponseUri.AbsoluteUri, ref Response, ref rqst, ++level);
            //    }
            //}
        }

        /// <summary>
        /// HTTP/HTTPS的GET方法获取HttpWebResponse
        /// </summary>
        /// <param name="Url">要GET数据的网址</param>
        /// <param name="headers">HTTP协议中消息报头所带的headers信息</param>
        /// <returns>HttpWebResponse</returns>
        protected void GetResponse(String Url, ref HttpWebResponse Response, ref HttpWebRequest rqst, String[] headers, int level = 0)
        {
            //如果使用代理https变为http
            //if (Url.ToLower().Contains("https://") && proxy.Address != null)
            //    Url = "http" + Url.Substring(5);
            Uri uri = new Uri(Url);
            //设置HTTPS方式
            if (Url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //构造HTTP头

            rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.KeepAlive = true;
            rqst.AllowAutoRedirect = false;
            rqst.ServicePoint.Expect100Continue = false;
            rqst.MaximumAutomaticRedirections = 3;
            rqst.UserAgent = UserAgent;
            if (reffer != null)
            {
                try
                {
                    rqst.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            foreach (String s in headers)
            {
                rqst.Headers.Add(s);
            }
            rqst.Timeout = timeOut;
            rqst.CookieContainer = Cookies;
            if (proxy.Address != null)
            {
                rqst.ProtocolVersion = HttpVersion.Version10;
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.Proxy = proxy;
            }
            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;
            //访问URL
            Response = (HttpWebResponse)rqst.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            //防止无限循环跳转
            if (level < 10)
            {
                //自动重定向网页

                if (Response.Headers["Location"] != null)
                {
                    Response.Close();
                    rqst.Abort();
                    String location = Response.Headers["Location"];
                    if (!location.Contains("://"))
                        location = uri.Scheme + "://" + uri.Authority + location;
                    GetResponse(location, ref Response, ref rqst, ++level);
                }
                else if (Response.ResponseUri != uri)
                {
                    Response.Close();
                    rqst.Abort();
                    GetResponse(Response.ResponseUri.AbsoluteUri, ref Response, ref rqst, ++level);
                }
            }
        }
        /// <summary>
        /// HTTP/HTTPS的POST方法获取HttpWebResponse
        /// </summary>
        /// <param name="Url">要POST数据的网址</param>
        /// <param name="PostData">POST的数据</param>
        /// <returns>HttpWebResponse</returns>
        protected void PostResponse(String Url, String PostData, ref HttpWebResponse Response, ref HttpWebRequest rqst, int level = 0)
        {
            //如果使用代理https变为http
            //if (Url.ToLower().Contains("https://") && proxy.Address != null)
            //    Url = "http" + Url.Substring(5);
            Uri uri = new Uri(Url);
            //设置HTTPS方式
            if (Url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //将POST数据按encoding编码转换为二进制数据
            byte[] bytes = encoding.GetBytes(PostData);
            //构造HTTP头

            rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.KeepAlive = true;
            rqst.AllowAutoRedirect = false;
            rqst.ServicePoint.Expect100Continue = false;
            rqst.Accept = "*/*";
            rqst.Headers.Add("Accept-Language: zh-CN");
            if (reffer != null)
            {
                try
                {
                    rqst.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            rqst.ContentType = "application/x-www-form-urlencoded";
            rqst.Headers.Add("Accept-Encoding: gzip,deflate");
            rqst.UserAgent = UserAgent;
            rqst.ContentLength = bytes.Length;
            rqst.Headers.Add("Cache-Control: no-cache");
            rqst.CookieContainer = Cookies;
            rqst.Method = "POST";
            if (proxy.Address != null)
            {
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.ProtocolVersion = HttpVersion.Version10;
                rqst.Proxy = proxy;
            }
            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;
            //POST数据
            using (Stream RequestStream = rqst.GetRequestStream())
            {
                RequestStream.Write(bytes, 0, bytes.Length);
                RequestStream.Close();
            }
            //获取POST结果
            Response = (HttpWebResponse)rqst.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            //防止无限循环跳转
            if (level < 5)
            {
                //自动重定向网页

                if (Response.Headers["Location"] != null)
                {
                    Response.Close();
                    rqst.Abort();
                    String location = Response.Headers["Location"];
                    if (!location.Contains("://"))
                        location = uri.Scheme + "://" + uri.Authority + location;
                    GetResponse(location, ref Response, ref rqst, ++level);
                }
                else if (Response.ResponseUri != uri)
                {
                    Response.Close();
                    rqst.Abort();
                    GetResponse(Response.ResponseUri.AbsoluteUri, ref Response, ref rqst, ++level);
                }
            }
        }
        /// <summary>
        /// HTTP/HTTPS的POST方法获取HttpWebResponse
        /// </summary>
        /// <param name="Url">要POST数据的网址</param>
        /// <param name="PostData">POST的数据</param>
        /// <returns>HttpWebResponse</returns>
        protected void PostResponse(String Url, String PostData, ref HttpWebResponse Response, ref HttpWebRequest rqst, String[] headers, int level = 0)
        {
            //如果使用代理https变为http
            //if (Url.ToLower().Contains("https://") && proxy.Address != null)
            //    Url = "http" + Url.Substring(5);
            Uri uri = new Uri(Url);
            //设置HTTPS方式
            if (Url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;
            //将POST数据按encoding编码转换为二进制数据
            byte[] bytes = encoding.GetBytes(PostData);
            //构造HTTP头

            rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.KeepAlive = true;
            rqst.ServicePoint.Expect100Continue = false;
            rqst.AllowAutoRedirect = false;
            rqst.Accept = "*/*";
            rqst.Headers.Add("Accept-Language: zh-CN");
            if (reffer != null)
            {
                try
                {
                    rqst.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            rqst.ContentType = "application/x-www-form-urlencoded";
            rqst.Headers.Add("Accept-Encoding: gzip,deflate");
            rqst.UserAgent = UserAgent;
            rqst.ContentLength = bytes.Length;
            rqst.Headers.Add("Cache-Control: no-cache");
            foreach (String s in headers)
            {
                rqst.Headers.Add(s);
            }
            rqst.CookieContainer = Cookies;
            rqst.Method = "POST";
            if (proxy.Address != null)
            {
                rqst.ProtocolVersion = HttpVersion.Version10;
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.Proxy = proxy;
            }
            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;
            //POST数据
            using (Stream RequestStream = rqst.GetRequestStream())
            {
                RequestStream.Write(bytes, 0, bytes.Length);
                RequestStream.Close();
            }
            //获取POST结果
            Response = (HttpWebResponse)rqst.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            //防止无限循环跳转
            if (level < 5)
            {
                //自动重定向网页

                if (Response.Headers["Location"] != null)
                {
                    reffer = null;
                    Response.Close();
                    rqst.Abort();
                    String location = Response.Headers["Location"];
                    if (!location.Contains("://"))
                        location = uri.Scheme + "://" + uri.Authority + location;
                    GetResponse(location, ref Response, ref rqst, ++level);
                }
                else if (Response.ResponseUri != uri)
                {
                    Response.Close();
                    rqst.Abort();
                    GetResponse(Response.ResponseUri.AbsoluteUri, ref Response, ref rqst, ++level);
                }
            }
        }
        /// <summary>
        /// 将Response数据流按encoding编码转换为字符串
        /// </summary>
        /// <param name="Response">HttpWebResponse</param>
        /// <returns>转换得到的字符串</returns>
        protected String HttpWebResponseToString(HttpWebResponse Response)
        {
            String Result = null;
            //将对方响应的数据流按指定编码还原为字符串
            using (Stream sm = Response.GetResponseStream())
            {
                Encoding cding = encoding;
                int ix = Response.ContentType.ToLower().IndexOf("charset=");
                if (ix != -1)
                {
                    try
                    {
                        cding = Encoding.GetEncoding(Response.ContentType.Substring(ix + "charset".Length + 1));
                    }
                    catch
                    {
                        cding = encoding;
                    }
                }
                //gzip压缩流需要先解压
                if (Response.ContentEncoding.ToLower().Contains("gzip"))
                {
                    using (GZipStream stmz = new GZipStream(sm, CompressionMode.Decompress))
                    {
                        Result = new StreamReader(stmz, cding).ReadToEnd();
                        stmz.Close();
                        stmz.Dispose();
                    }
                }
                else
                    Result = new StreamReader(sm, cding).ReadToEnd();
                sm.Close();
                sm.Dispose();
            }
            return Result;
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="uploadfile">上传文件路径</param>
        /// <param name="url">URL地址</param>
        /// <param name="fileFormName">文件类型</param>
        /// <param name="contenttype">内容</param>
        /// <param name="addin">附加结尾内容</param>
        /// <returns></returns>
        protected void UploadFileEx(String uploadfile, String url, String fileFormName, String contenttype, ref HttpWebResponse Response, ref HttpWebRequest webrequest, String addin = "")
        {
            if (string.IsNullOrEmpty(fileFormName))
            {
                fileFormName = "file";
            }

            if (string.IsNullOrEmpty(contenttype))
            {
                contenttype = "application/octet-stream";
            }

            Uri uri = new Uri(url);

            //设置HTTPS方式
            if (url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            String boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.KeepAlive = false;
            webrequest.CookieContainer = Cookies;
            webrequest.Accept = "*/*";
            webrequest.Headers.Add("Accept-Encoding: gzip, deflate");
            webrequest.Headers.Add("Accept-Language: zh-CN");
            webrequest.UserAgent = UserAgent;
            webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
            webrequest.Method = "POST";
            if (reffer != null)
            {
                try
                {
                    webrequest.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            webrequest.Timeout = 600000;
            if (proxy.Address != null)
            {
                //启用代理
                webrequest.UseDefaultCredentials = true;
                webrequest.Proxy = proxy;
            }

            // Build up the post message header

            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(boundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append(fileFormName);
            sb.Append("\"; filename=\"");
            sb.Append(Path.GetFileName(uploadfile));
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append(contenttype);
            sb.Append("\r\n");
            sb.Append("\r\n");

            String postHeader = sb.ToString();
            byte[] postHeaderBytes = encoding.GetBytes(postHeader);

            StringBuilder sb2 = new StringBuilder();
            sb2.Append("\r\n--" + boundary + "\r\n");
            if (!String.IsNullOrEmpty(addin))
            {
                String[] addinArray = addin.Split('&');
                foreach (String a in addinArray)
                {
                    sb2.Append(a);
                    sb2.Append("\r\n--" + boundary + "\r\n");
                }
            }
            // Build the trailing boundary String as a byte array

            // ensuring the boundary appears on a line by itself

            byte[] boundaryBytes =
                   Encoding.ASCII.GetBytes(sb2.ToString());



            using (FileStream fileStream = new FileStream(uploadfile, FileMode.Open, FileAccess.Read))
            {
                long length = postHeaderBytes.Length + fileStream.Length +
                                                       boundaryBytes.Length;
                webrequest.ContentLength = length;

                using (Stream requestStream = webrequest.GetRequestStream())
                {
                    // Write out our post header

                    requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                    // Write out the file contents

                    byte[] buffer = new Byte[checked((uint)Math.Min(4096,
                                             (int)fileStream.Length))];
                    int bytesRead = 0;
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                        requestStream.Write(buffer, 0, bytesRead);

                    // Write out the trailing boundary

                    requestStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                }
                //fileStream.Close();
                //fileStream.Dispose();
            }
            Response = (HttpWebResponse)webrequest.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pic"></param>
        /// <param name="url"></param>
        /// <param name="Response"></param>
        /// <param name="webrequest"></param>
        protected void UploadFile(byte[] pic, string url, ref HttpWebResponse Response, ref HttpWebRequest webrequest)
        {
            Uri uri = new Uri(url);

            //设置HTTPS方式
            if (url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            webrequest = (HttpWebRequest)WebRequest.Create(uri);
            webrequest.KeepAlive = false;
            webrequest.CookieContainer = Cookies;
            webrequest.Accept = "*/*";
            webrequest.Headers.Add("Accept-Encoding: gzip, deflate");
            webrequest.Headers.Add("Accept-Language: zh-CN");
            webrequest.UserAgent = UserAgent;
            webrequest.ContentType = "application/octet-stream";
            webrequest.Method = "POST";
            if (reffer != null)
            {
                try
                {
                    webrequest.Referer = reffer.AbsoluteUri;
                }
                catch { }
            }
            webrequest.Timeout = 600000;
            if (proxy.Address != null)
            {
                //启用代理
                webrequest.UseDefaultCredentials = true;
                webrequest.Proxy = proxy;
            }

            using (Stream requestStream = webrequest.GetRequestStream())
            {
                requestStream.Write(pic, 0, pic.Length);
            }

            Response = (HttpWebResponse)webrequest.GetResponse();

            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
        }

        #region 公共方法
        /// <summary>
        /// HTTP/HTTPS的POST方法
        /// </summary>
        /// <param name="Url">要POST数据的网址,Url须包括"http://"或"https://"</param>
        /// <param name="PostData">POST的数据</param>
        /// <returns>POST返回的结果
        /// 返回null 查看error中的详细异常情况</returns>
        public String Post(String Url, String PostData)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                PostResponse(Url, PostData, ref rsps, ref rqst);
                //将对方响应的数据流按指定编码还原为字符串
                Response = HttpWebResponseToString(rsps);
                rsps.Close();
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }

        /// <summary>
        /// Post Img 到新浪微博
        /// </summary>
        /// <param name="fileContent"></param>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        /// <param name="contentType"></param>
        /// <param name="paramName"></param>
        /// <param name="nvc"></param>
        /// <returns></returns>
        public string UploadSinaWeiboImage(byte[] fileContent, string fileName, string url, string contentType = "image/jpeg", string paramName = "pic", NameValueCollection nvc = null)
        {
            if (url.ToLower().Contains("https://"))
                ServicePointManager.ServerCertificateValidationCallback = CheckValidationResult;

            Uri uri = new Uri(url);
            HttpWebRequest rqst = (HttpWebRequest)WebRequest.Create(uri);
            rqst.Headers.Add("Accept-Encoding: gzip,deflate");
            rqst.Headers.Add("Accept-Language: zh-CN");
            rqst.Headers.Add("Cache-Control: no-cache");
            rqst.Headers.Add("X-Requested-With: XMLHttpRequest");

            rqst.UserAgent = UserAgent;
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            rqst.ContentType = "multipart/form-data; boundary=" + boundary;
            rqst.Accept = "*/*";
            rqst.Method = "POST";
            rqst.CookieContainer = Cookie;
            rqst.KeepAlive = true;
            rqst.AllowAutoRedirect = false;
            rqst.ServicePoint.Expect100Continue = false;
            if (reffer != null)
            {
                rqst.Referer = reffer.AbsoluteUri;
            }
            if (proxy.Address != null)
            {
                rqst.ProtocolVersion = HttpVersion.Version10;
                //启用代理
                rqst.UseDefaultCredentials = true;
                rqst.Proxy = proxy;
            }

            Stream rs = rqst.GetRequestStream();
            const string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            if (nvc == null)
            {
                nvc = new NameValueCollection { { "type", "json" } };
            }

            byte[] boundarybytes = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            const string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, fileName, contentType);
            byte[] headerbytes = Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            MemoryStream fileStream = new MemoryStream(fileContent);
            byte[] buffer = new byte[4096];
            int bytesRead;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            //记录当前访问页以便下次访问设置Reffer
            reffer = uri;

            //获取POST结果
            HttpWebResponse Response = (HttpWebResponse)rqst.GetResponse();
            //更新COOKIE
            foreach (Cookie ck in Response.Cookies)
            {
                Cookies.Add(ck);
            }
            return HttpWebResponseToString(Response);
        }

        public string UploadImage(byte[] pic, string url, string fileFormName, string contenttype, string addin)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                using (MemoryStream s = new MemoryStream(pic))
                {
                    using (Image img = Image.FromStream(s))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            String tempPath = Path.GetTempPath();
                            String filename = tempPath + new Random(Guid.NewGuid().GetHashCode()).Next(3000, 5000).ToString() + ".jpg";
                            img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            UploadFileEx(filename, url, fileFormName, contenttype, ref rsps, ref rqst, addin);
                            //将对方响应的数据流按指定编码还原为字符串
                            Response = HttpWebResponseToString(rsps);
                            Response = Response + "ResponseURL=" + rsps.ResponseUri.AbsoluteUri;
                            File.Delete(filename);
                            stream.Close();
                            //stream.Dispose();
                        }
                        //img.Dispose();
                    }
                    s.Close();
                    //s.Dispose();
                }
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }

        /// <summary>
        /// HTTP/HTTPS的POST方法
        /// </summary>
        /// <param name="Url">要POST数据的网址,Url须包括"http://"或"https://"</param>
        /// <param name="PostData">POST的数据</param>
        /// <returns>POST返回的结果

        /// 返回null 查看error中的详细异常情况</returns>
        public String PostWithHeaders(String Url, String PostData, String[] headers)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                PostResponse(Url, PostData, ref rsps, ref rqst, headers);
                //将对方响应的数据流按指定编码还原为字符串
                Response = HttpWebResponseToString(rsps);
                rsps.Close();
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的GET网页方法
        /// </summary>
        /// <param name="Url">要GET网页的网址,Url须包括"http://"或"https://"</param>
        /// <returns>GET到的HTML
        /// 返回null 查看error中的详细异常情况</returns>
        public String GetHTML(String Url)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse(Url, ref rsps, ref rqst);
                //将对方响应的数据流按指定编码还原为字符串
                Response = HttpWebResponseToString(rsps);
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的GET网页方法
        /// </summary>
        /// <param name="Url">要GET网页的网址,Url须包括"http://"或"https://"</param>
        /// <returns>GET到的HTML
        /// 返回null 查看error中的详细异常情况</returns>
        public String GetHTML1(String Url)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse1(Url, ref rsps, ref rqst);
                //将对方响应的数据流按指定编码还原为字符串
                Response = HttpWebResponseToString(rsps);
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的GET网页方法
        /// </summary>
        /// <param name="Url">要GET网页的网址,Url须包括"http://"或"https://"</param>
        /// <param name="headers">HTTP协议中消息报头所带的headers信息</param>
        /// <returns>GET到的HTML
        /// 返回null 查看error中的详细异常情况</returns>
        public String GetHTMLWithHeaders(String Url, String[] headers)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse(Url, ref rsps, ref rqst, headers);
                //将对方响应的数据流按指定编码还原为字符串
                Response = HttpWebResponseToString(rsps);
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的GET图片方法
        /// </summary>
        /// <param name="Url">要GET图片的网址,Url须包括"http://"或"https://"</param>
        /// <returns>GET到的图片
        /// 返回null 查看error中的详细异常情况</returns>
        public Image GetImage(String Url)
        {
            Image Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse(Url, ref rsps, ref rqst);
                //将对方响应的数据流还原为图片
                using (Stream sm = rsps.GetResponseStream())
                {
                    Response = Image.FromStream(sm);
                    sm.Close();
                    sm.Dispose();
                }
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的GET图片方法
        /// </summary>
        /// <param name="Url">要GET图片的网址,Url须包括"http://"或"https://"</param>
        /// <returns>GET到的图片转换为Base64 String的结果
        /// 返回null 查看error中的详细异常情况</returns>
        public String GetImageString(String Url)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse(Url, ref rsps, ref rqst);
                //将对方响应的数据流还原为图片
                using (Stream inStream = rsps.GetResponseStream())
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int l;
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);
                        byte[] bytes = new byte[outStream.Length];
                        outStream.Position = 0;
                        outStream.Read(bytes, 0, (int)outStream.Length);
                        Response = Convert.ToBase64String(bytes);
                        outStream.Close();
                        outStream.Dispose();
                    }
                    inStream.Close();
                    inStream.Dispose();
                }
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }

        public byte[] GetImageByte(String Url)
        {
            byte[] Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                //访问URL
                GetResponse(Url, ref rsps, ref rqst);
                //将对方响应的数据流还原为图片
                using (Stream inStream = rsps.GetResponseStream())
                {
                    using (MemoryStream outStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[1024];
                        int l;
                        do
                        {
                            l = inStream.Read(buffer, 0, buffer.Length);
                            if (l > 0)
                                outStream.Write(buffer, 0, l);
                        }
                        while (l > 0);
                        byte[] bytes = new byte[outStream.Length];
                        outStream.Position = 0;//新浪通行证专用15   原始值0
                        outStream.Read(bytes, 0, (int)outStream.Length);
                        Response = bytes;
                    }
                }
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }
        /// <summary>
        /// HTTP/HTTPS的图片上传方法
        /// </summary>
        /// <param name="imgstr">要上传的图片 Base64 String格式</param>
        /// <param name="url">要上传图片的网址,Url须包括"http://"或"https://"</param>
        /// <param name="fileFormName">文件类型</param>
        /// <param name="contenttype">内容</param>
        /// <param name="addin">附加结尾内容</param>
        /// <returns>图片上传后返回的结果
        /// 返回null 查看error中的详细异常情况</returns>
        public String UploadImageString(String imgstr, String url, String fileFormName, String contenttype, String addin)
        {
            String Response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                byte[] imgbuffer = Convert.FromBase64String(imgstr);
                using (MemoryStream s = new MemoryStream(imgbuffer))
                {
                    using (Image img = Image.FromStream(s))
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            String tempPath = Path.GetTempPath();
                            String filename = tempPath + new Random(Guid.NewGuid().GetHashCode()).Next(3000, 5000).ToString() + ".jpg";
                            img.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                            UploadFileEx(filename, url, fileFormName, contenttype, ref rsps, ref rqst, addin);
                            //将对方响应的数据流按指定编码还原为字符串
                            Response = HttpWebResponseToString(rsps);
                            Response = Response + "ResponseURL=" + rsps.ResponseUri.AbsoluteUri;
                            File.Delete(filename);
                            stream.Close();
                            //stream.Dispose();
                        }
                        //img.Dispose();
                    }
                    s.Close();
                    //s.Dispose();
                }
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return Response;
        }

        public string UploadImage(byte[] pic, string url)
        {
            string response = null;
            HttpWebResponse rsps = null;
            HttpWebRequest rqst = null;
            try
            {
                UploadFile(pic, url, ref rsps, ref rqst);
                //将对方响应的数据流按指定编码还原为字符串
                response = HttpWebResponseToString(rsps);
                response = response + "ResponseURL=" + rsps.ResponseUri.AbsoluteUri;
            }
            catch (WebException e)
            {
                error = e;
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                if (rsps != null)
                    rsps.Close();
                if (rqst != null)
                    rqst.Abort();
            }
            return response;
        }

        /// <summary>
        /// 提取文本中满足某正则式的文字
        /// </summary>
        /// <param name="text">要查找的文本</param>
        /// <param name="pattern">正则式</param>
        /// <returns>返回所有匹配的文字</returns>
        public List<String> GetSpecialText(String text, String pattern)
        {
            List<String> texts = new List<String>();
            try
            {
                Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                Match mc = regex.Match(text);
                while (mc.Success)
                {
                    texts.Add(mc.Groups[0].Value);
                    mc = mc.NextMatch();
                }
            }
            catch (Exception e)
            {
                error = e;
            }
            return texts;
        }
        /// <summary>
        /// 在指定的输入字符串内，使用指定的替换字符串替换与某个正则表达式模式匹配的所有字符串。

        /// </summary>
        /// <param name="text">要搜索匹配项的字符串</param>
        /// <param name="pattern">正则式</param>
        /// <param name="replacement">替换字符串</param>
        /// <returns>一个与输入字符串基本相同的新字符串，唯一的差别在于，其中的每个匹配字符串已被替换字符串代替。

        /// 如果未替换则返回原始字符串text</returns>
        public String ReplaceSpecialText(String text, String pattern, String replacement)
        {
            String texts = text;
            try
            {
                Regex regex = new Regex(pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase);
                texts = regex.Replace(text, replacement);
            }
            catch (Exception e)
            {
                error = e;
            }
            return texts;
        }
        /// <summary>
        /// 清空已保存的Cookie
        /// </summary>
        public void CookieClear()
        {
            this.Cookies = new CookieContainer();
        }
        /// <summary>
        /// 获取转换为字符串格式的CookieContainer
        /// </summary>
        /// <returns></returns>
        public String GetCookie()
        {
            String result = String.Empty;
            try
            {
                List<Cookie> lstCookies = new List<Cookie>();
                Hashtable table = (Hashtable)Cookie.GetType().InvokeMember("m_domainTable",
                    BindingFlags.NonPublic | BindingFlags.GetField |
                    BindingFlags.Instance, null, Cookie, new object[] { });
                foreach (object pathList in table.Values)
                {
                    SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                        BindingFlags.NonPublic | BindingFlags.GetField
                        | BindingFlags.Instance, null, pathList, new object[] { });
                    foreach (CookieCollection colCookies in lstCookieCol.Values)
                        foreach (Cookie c in colCookies) lstCookies.Add(c);
                }
                StringBuilder sbc = new StringBuilder();
                foreach (Cookie cookie in lstCookies)
                {
                    sbc.AppendFormat("{0};{1};{2};{3};{4};{5}\r\n",
                        cookie.Domain, cookie.Name, cookie.Path, cookie.Port,
                        cookie.Secure.ToString(), cookie.Value);
                }
                result = sbc.ToString();
            }
            catch (Exception e)
            {
                error = e;
            }
            return result;
        }
        /// <summary>
        /// 将转换为字符串格式的CookieContainer设置为当前CookieContainer
        /// </summary>
        /// <param name="cookieStr">转换为字符串格式的CookieContainer</param>
        /// <returns>成功或失败</returns>
        public bool SetCookie(String cookieStr)
        {
            bool result = false;
            try
            {
                CookieClear();
                String[] cookies = cookieStr.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (String c in cookies)
                {
                    String[] cc = c.Split(";".ToCharArray());
                    Cookie ck = new Cookie();
                    ck.Discard = false;
                    ck.Domain = cc[0];
                    ck.Expired = false;
                    ck.Expires = new DateTime(2099, 12, 31);
                    ck.HttpOnly = true;
                    ck.Name = cc[1];
                    ck.Path = cc[2];
                    //ck.Port = cc[3];
                    ck.Secure = bool.Parse(cc[4]);
                    ck.Value = cc[5];
                    Cookie.Add(ck);
                }
            }
            catch (Exception e)
            {
                error = e;
            }
            return result;
        }
        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="proxyUrl">代理地址，以http://开头,为空表示取消代理</param>
        /// <param name="username">代理用户名，为空表示匿名代理</param>
        /// <param name="password">代理密码</param>
        public void SetProxy(String proxyUrl, String username, String password)
        {
            try
            {
                if (!String.IsNullOrWhiteSpace(proxyUrl))
                {
                    proxy.Address = new Uri(proxyUrl);              //设置代理地址
                    if (!String.IsNullOrWhiteSpace(username))
                        proxy.Credentials = new NetworkCredential(username, password);      //设置代理用户名密码

                    else
                        proxy.Credentials = CredentialCache.DefaultCredentials;
                }
                else
                {
                    proxy.Address = null;
                    proxy.Credentials = null;
                }
            }
            catch (Exception e)
            {
                error = e;
            }
        }
        #endregion
    }
}
