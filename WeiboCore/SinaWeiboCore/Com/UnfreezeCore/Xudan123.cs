using System;
using System.Text.RegularExpressions;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    /// <summary>
    /// 飞Q平台
    /// </summary>
    public class Xudan123 : ISmsapi
    {
        readonly WebAccessBase web;
        private const string url = "http://116.255.240.180/do.aspx";
        readonly string uid;
        readonly string token;

        public Xudan123(string uid, string pwd)
        {
            web = new WebAccessBase {TimeOut = 60000};
            string postData = string.Format("action=loginIn&uid={0}&pwd={1}", uid, pwd);
            string html = web.Post(url, postData);
            string str = @"^" + uid + @"\|[0-9a-f]{32}$";
            Regex reg = new Regex(str);
            if (!string.IsNullOrEmpty(html) && reg.IsMatch(html))
            {
                token = html.Split('|')[1];
                this.uid = uid;
            }
            else
                throw new Exception("飞Q平台初始化失败");
        }

        public string GetMobile(string type = "48")
        {
            web.Reffer = null;
            string postData = string.Format("action=getMobilenum&pid={0}&uid={1}&token={2}", type, uid, token);
            string html = web.Post(url, postData);
            Regex reg = new Regex(@"^(?<mobile>\d{11})\|[0-9a-f]{32}$");
            if (!string.IsNullOrEmpty(html) && reg.IsMatch(html))
                return reg.Match(html).Groups["mobile"].Value;
            else
                return null;
        }

        public string Unlock(string mobile, string type = "")
        {
            string postData = string.Format("action=getVcodeAndReleaseMobile&uid={0}&token={1}&mobile={2}", uid, token, mobile);
            string html = web.Post(url, postData);
            if (!string.IsNullOrEmpty(html))
            {
                //"您本次操作的验证码是[779885]（20分钟内有效），请尽快完成验证。【本条免费】【新浪】"
                Regex regex = new Regex(@"\b(?<verified>\d{6})\b");
                if (regex.IsMatch(html))
                    return regex.Match(html).Groups["verified"].Value;
                else
                    return html;
            }
            else
                return "";
        }

        public void AddIgnoreList(string mobile, string type = "48")
        {
            web.Reffer = null;
            var postData = string.Format("action=addIgnoreList&pid={0}&mobiles={3}&uid={1}&token={2}", type, uid, token, mobile);
            web.Post(url, postData);
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
            web.Reffer = null;
            string postData = string.Format("action=cancelSMSRecvAll&uid={0}&token={1}", uid, token);
            web.Post(url, postData);
        }
    }
}