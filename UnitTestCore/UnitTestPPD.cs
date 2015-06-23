using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
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
                }
            };
            HttpResult result = http.GetHtml(item);
            string cookie = result.Cookie;
            Dictionary<string, string> cks = new Dictionary<string, string>();
            foreach (string source in cookie.Split(';').Where(c => !string.IsNullOrEmpty(c)))
            {
                if (source.Split('=').Count() > 2)
                {
                    foreach (string ssource in source.Split(','))
                    {
                        string[] ss = ssource.Split('=');
                        string key = ss[0].Trim().Split(',').Last();

                        if (ss.Count() == 2 && !cks.ContainsKey(key.Trim()))
                        {
                            cks.Add(key.Trim(), ss[1].Trim());
                        }
                    }
                }
                else
                {
                    string[] ss = source.Split('=');
                    string key = ss[0].Trim().Split(',').Last();

                    if (ss.Count() == 2 && !cks.ContainsKey(key.Trim()))
                    {
                        cks.Add(key.Trim(), ss[1].Trim());
                    }
                }
            }
            string uniqueid = cks["uniqueid"];
            string dtNow = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");
            cookie =
                string.Format(
                    "uniqueid={0}; regSourceId=0; referID=0; fromUrl=; referDate={1}; currentUrl=https%3A%2F%2Fac.ppdai.com%2Fuser%2Flogin",
                    uniqueid,
                    HttpUtility.UrlEncode(dtNow));

            item = item.GetNextHttpItem("https://ac.ppdai.com/User/Login", cookie);
            item.Header.Add("X-Requested-With: XMLHttpRequest");
            item.ContentType = "application/x-www-form-urlencoded";
            item.Referer = "https://ac.ppdai.com/User/Login";
            item.Method = "POST";
            item.Postdata = string.Format("IsAsync=true&Redirect=&UserName={0}&Password={1}&RememberMe=false", "funny_zhoulin%40163.com", "");

            result = http.GetHtml(item);

            cookie = result.Cookie;
            foreach (string source in cookie.Split(';').Where(c => !string.IsNullOrEmpty(c)))
            {
                if (source.Split('=').Count() > 2)
                {
                    foreach (string ssource in source.Split(','))
                    {
                        string[] ss = ssource.Split('=');
                        string key = ss[0].Trim().Split(',').Last();

                        if (ss.Count() == 2 && !cks.ContainsKey(key.Trim()))
                        {
                            cks.Add(key.Trim(), ss[1].Trim());
                        }
                    }
                }
                else
                {
                    string[] ss = source.Split('=');
                    string key = ss[0].Trim().Split(',').Last();
                    if (ss.Count() == 2 && !cks.ContainsKey(key.Trim()))
                    {
                        cks.Add(key.Trim(), ss[1].Trim());
                    }
                }
            }

            cookie = string.Join(";", cks.Select(c => c.Key + "=" + c.Value));

            item = item.GetNextHttpItem("http://invest.ppdai.com/PaiMoney/PaiMoneySignin", cookie);
            item.Accept = "*";
            item.ContentType = "";
            item.Referer = "http://invest.ppdai.com/account/lend";
            item.Postdata = "";

            result = http.GetHtml(item);
        }
    }
}
