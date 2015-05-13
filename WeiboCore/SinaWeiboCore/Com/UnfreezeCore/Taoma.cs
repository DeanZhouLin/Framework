using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    internal class Taoma : ISmsapi
    {
        private const string Url = "http://api.taomapt.com/http.do";

        private readonly string _username;

        private readonly string _passwordMd5;

        private readonly WebAccessBase _web;

        public Taoma(string username, string password)
        {
            _username = username;
            _passwordMd5 = Md5Encrypt(password);
            _web = new WebAccessBase { TimeOut = 60000 };
        }

        private static string Md5Encrypt(string beforeStr)
        {
            try
            {
                MD5 md5 = MD5.Create();
                byte[] hashs = md5.ComputeHash(Encoding.UTF8.GetBytes(beforeStr));
                return hashs.Aggregate("", (current, b) => current + b.ToString("x2"));
            }
            catch
            {
                return null;
            }
        }

        public string GetMobile(string type = "")
        {
            string url = string.Format("{0}?action=getMobiles&userID={1}&password={2}&projectID={3}&size=1", Url, _username, _passwordMd5, type);

            _web.Reffer = null;

            var html = _web.GetHTML(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            try
            {
                var json = DynamicJson.Parse(html);
                return json.state ? json.mobiles[0] : null;
            }
            catch
            {
                return null;
            }
        }

        public string Unlock(string mobile, string type = "")
        {
            string url = string.Format("{0}?action=getMessages&userID={1}&password={2}&mobile={4}&projectID={3}&softAuthor=leodaping", Url, _username, _passwordMd5, type, mobile);

            _web.Reffer = null;

            var html = _web.GetHTML(url);
            if (string.IsNullOrEmpty(html))
            {
                return "";
            }

            try
            {
                var json = DynamicJson.Parse(html);
                if (json.state)
                {
                    string msg = json.msg;
                    return Regex.Match(msg, @"\b(?<verified>\d{6})\b").Groups[1].Value;
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        public void CancelSmsRecvAll(int count = 1)
        {
            throw new NotImplementedException();
        }

        public void AddIgnoreList(string mobile, string type = "")
        {
            for (int i = 0; i < 20; i++)
            {
                string url = string.Format("{0}?action=addBlackMobiles&userID={1}&password={2}&projectID={3}&mobile={4}", Url, _username, _passwordMd5, type, mobile);
                _web.Reffer = null;
                var html = _web.GetHTML(url);
                if (!string.IsNullOrEmpty(html))
                {
                    try
                    {
                        var json = DynamicJson.Parse(html);
                        if (json.state)
                            return;
                        
                        if (json.message != "操作频繁")
                            return;
                    }
                    catch (Exception exception)
                    {
                        Debug.Write(exception);
                    }
                }
                Thread.Sleep(3000);
            }
        }

        public string SendSms(string mobile, string type, string content)
        {
            throw new NotImplementedException();
        }

        public string GetSmsStatus(string mobile, string type)
        {
            throw new NotImplementedException();
        }
    }
}
