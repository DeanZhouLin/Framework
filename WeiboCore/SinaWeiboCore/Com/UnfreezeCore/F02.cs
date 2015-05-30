using System;
using System.Text.RegularExpressions;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    /// <summary>
    /// 爱码平台
    /// </summary>
    public class F02 : ISmsapi
    {
        readonly WebAccessBase web;
        private const string url = "http://api.f02.cn:8888/http.do";
        readonly string uid;
        readonly string token;

        public F02(string uid, string pwd)
        {
            web = new WebAccessBase { TimeOut = 60000 };
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
                throw new Exception("爱码初始化失败");
        }

        public string GetMobile(string type)
        {
            web.Reffer = null;
            string postData = string.Format("action=getMobilenum&pid={0}&uid={1}&token={2}", type, uid, token);
            string html = web.Post(url, postData);
            Regex reg = new Regex(@"^(?<mobile>\d{11})\|[0-9a-f]{32}$");
            if (!string.IsNullOrEmpty(html) && reg.IsMatch(html))
                return reg.Match(html).Groups["mobile"].Value;
            //File.AppendAllText(System.Environment.CurrentDirectory + "/GetMobile_html.txt", DateTime.Now + "\t" + html + Environment.NewLine);
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
                return html;
            }
            return "";
        }

        public void AddIgnoreList(string mobile, string type)
        {
            web.Reffer = null;
            var postData = string.Format("action=addIgnoreList&pid={0}&mobiles={3}&uid={1}&token={2}", type, uid, token, mobile);
            web.Post(url, postData);
        }

        public string SendSms(string mobile, string type, string content)
        {
            web.Reffer = null;
            string postData = string.Format("action=sendSms&uid={0}&token={1}&mobile={2}&pid={3}&content={4}", uid, token, mobile, type, content);
            var html = web.Post(url, postData);
            return html;
        }

        public string GetSmsStatus(string mobile, string type)
        {
            web.Reffer = null;
            string postData = string.Format("action=getSmsStatus&uid={0}&token={1}&mobile={2}&pid={3}", uid, token, mobile, type);
            var html = web.Post(url, postData);
            return html;
        }

        public void CancelSmsRecvAll(int count = 1)
        {
            web.Reffer = null;
            string postData = string.Format("action=cancelSMSRecvAll&uid={0}&token={1}", uid, token);
            web.Post(url, postData);
        }
    }
}
