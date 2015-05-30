using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    public class JikeSms : ISmsapi
    {
        private static string HeConToUe(string content)
        {
            return System.Web.HttpUtility.UrlEncode(System.Web.HttpUtility.HtmlDecode(content), Encoding.UTF8);
        }

        private static string Md5Encrypt(string beforeStr)
        {
            string afterString = "";
            try
            {
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] hashs = md5.ComputeHash(Encoding.UTF8.GetBytes(beforeStr));

                //这里是字母加上数据进行加密.//3y 可以,y3不可以或 x3j等应该是超过32位不可以
                afterString = hashs.Aggregate(afterString, (current, @by) => current + @by.ToString("x2"));
            }
            catch
            {
                // ignored
            }
            return afterString;
        }

        readonly WebAccessBase _web;


        public JikeSms(string uid, string pwd)
        {
            _web = new WebAccessBase { TimeOut = 60000 };
            if (!Login(uid, pwd))
                throw new Exception("初始化JikeSMS失败");
        }

        private bool Login(string uid, string pwd)
        {
            const string url = "http://www.jikesms.com/common/ajax.htm";
            string postdata = string.Format("action={2}&event_name_login={3}&uid={0}&password={1}", uid, Md5Encrypt(pwd).ToUpper(), HeConToUe("user: UserEventAction"), HeConToUe("提交"));
            string html = _web.Post(url, postdata);
            if (!string.IsNullOrEmpty(html) && html.Contains("登录成功"))
                return true;
            return false;
        }

        public string GetMobile(string type = "6")
        {
            _web.Reffer = null;
            const string url = "http://www.jikesms.com/common/ajax.htm";
            string postdata = string.Format("event_name_getPhone=%E5%8F%96%E6%89%8B%E6%9C%BA%E5%8F%B7&action=phone%3APhoneEventAction&serviceId={0}", type);
            string html = _web.Post(url, postdata);
            return !string.IsNullOrEmpty(html) ? Regex.Match(html, @"(\d{11})", RegexOptions.Multiline).Groups[1].Value : null;
        }

        public string Unlock(string mobile, string type = "6")
        {
            _web.Reffer = null;
            const string url = "http://www.jikesms.com/common/ajax.htm";
            string postdata = string.Format("event_name_getMessage=%E5%8F%96%E7%9F%AD%E4%BF%A1&action=phone%3APhoneEventAction&serviceId={1}&phone={0}", mobile, type);
            string html = _web.Post(url, postdata);
            if (string.IsNullOrEmpty(html)) return "";
            //"您本次操作的验证码是[779885]（20分钟内有效），请尽快完成验证。【本条免费】【新浪】"
            Regex regex = new Regex(@"\b(?<verified>\d{6})\b");
            return regex.IsMatch(html) ? regex.Match(html).Groups["verified"].Value : html;
        }

        public void AddIgnoreList(string mobile, string type = "6")
        {
            _web.Reffer = null;
            const string url = "http://www.jikesms.com/common/ajax.htm";
            string postdata = string.Format("event_name_addBlacklist=%E6%B7%BB%E5%8A%A0%E9%BB%91%E5%90%8D%E5%8D%95&action=phone%3APhoneEventAction&serviceId={1}&phone={0}&forever=false", mobile, type);
            _web.Post(url, postdata);
        }

        public string SendSms(string mobile, string type, string content)
        {
            throw new NotImplementedException();
        }

        public string GetSmsStatus(string mobile, string type)
        {
            throw new NotImplementedException();
        }


        public void CancelSmsRecvAll(int count = 1)
        {
            _web.Reffer = null;
            const string url = "http://www.jikesms.com/common/ajax.htm";
            const string postdata = "event_name_cancelAllRecv=%E9%87%8A%E6%94%BE%E6%89%80%E6%9C%89%E6%89%8B%E6%9C%BA%E5%8F%B7&action=phone%3APhoneEventAction";
            _web.Post(url, postdata);
        }
    }
}