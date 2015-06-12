using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeanZhou.Framework;

namespace UnitTestCore
{
    [TestClass]
    public class UnitTestPPD
    {
        [TestMethod]
        public void TestLogin()
        {
            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "https://ac.ppdai.com/User/Login",//URL     必需项
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36",
                Header = new WebHeaderCollection
                {
                    {"Accept-Encoding"," gzip, deflate, sdch"},
                    {"Accept-Language","zh-CN,zh;q=0.8"}
                },
                Allowautoredirect = true
            };
            HttpResult result = http.GetHtml(item);
            string cookie = result.Cookie;

            item = new HttpItem
            {
                URL = "https://ac.ppdai.com/User/Login",//URL     必需项
                Cookie = cookie,
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36",
                Header = new WebHeaderCollection
                {
                    {"Accept-Encoding"," gzip, deflate"},
                    {"Accept-Language","zh-CN,zh;q=0.8"},
                    {"X-Requested-With","XMLHttpRequest"}
                },
                ContentType = "application/x-www-form-urlencoded",
                Referer = "https://ac.ppdai.com/User/Login",
                Method = "POST",
                Postdata = "IsAsync=true&Redirect=&UserName={0}&Password={1}&RememberMe=false"
            };
            result = http.GetHtml(item);

            cookie = result.Cookie;
            item = new HttpItem
            {
                URL = "http://invest.ppdai.com/PaiMoney/PaiMoneySignin",
                Cookie = cookie,
                Accept = "*",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36",
                Header = new WebHeaderCollection
                {
                    {"Accept-Encoding"," gzip, deflate"},
                    {"Accept-Language","zh-CN,zh;q=0.8"},
                    {"X-Requested-With","XMLHttpRequest"}
                },
                ContentType = "",
                Referer = "http://invest.ppdai.com/account/lend",
                Method = "POST",
                Postdata = ""
            };
            result = http.GetHtml(item);
        }
    }
}
