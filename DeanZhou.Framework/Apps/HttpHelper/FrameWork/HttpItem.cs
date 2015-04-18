using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DeanZhou.Framework
{
    /// <summary>
    /// Http请求参考类
    /// </summary>
    public class HttpItem
    {
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout { get; set; }

        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType { get; set; }

        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; }

        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }

        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }

        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie { get; set; }

        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; }

        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; }

        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public bool IsToLower { get; set; }

        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public bool Allowautoredirect { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit { get; set; }

        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; }

        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd { get; set; }

        /// <summary>
        /// 代理 服务IP
        /// </summary>
        public string ProxyIp { get; set; }

        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType { get; set; }

        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }

        /// <summary>
        /// 获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。
        /// 如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue { get; set; }

        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }

        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; set; }

        public HttpItem()
        {
            Postdata = string.Empty;
            PostDataType = PostDataType.String;
            UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            ContentType = "text/html";
            Accept = "text/html, application/xhtml+xml, */*";
            ReadWriteTimeout = 30000;
            Timeout = 100000;
            Method = "GET";
            Expect100Continue = true;
            Header = new WebHeaderCollection();
            ResultType = ResultType.String;
            Connectionlimit = 1024;
            CerPath = string.Empty;
            Referer = string.Empty;
            Cookie = string.Empty;
            URL = string.Empty;
            Allowautoredirect = false;
            IsToLower = false;
            CookieCollection = null;
            Encoding = null;
        }
    }
}
