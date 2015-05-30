using System;
using System.Text.RegularExpressions;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    public class Yzm1 : ISmsapi
    {
        readonly WebAccessBase web;
        private const string url = "http://www.yzm1.com/api/do.php";
        readonly string token;

        public Yzm1(string username, string password)
        {
            web = new WebAccessBase { TimeOut = 60000 };
            string postData = string.Format("action=loginIn&name={0}&password={1}", username, password);
            string html = web.Post(url, postData);
            const string str = @"1\|[A-Fa-f0-9]{8}(-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}$";
            Regex reg = new Regex(str);
            if (!string.IsNullOrEmpty(html) && reg.IsMatch(html))
            {
                token = html.Split('|')[1];
            }
            else
                throw new Exception("壹码初始化失败");
        }

        public string GetMobile(string type)
        {
            web.Reffer = null;
            string postData = string.Format("action=getPhone&sid={0}&token={1}", type, token);
            string html = web.Post(url, postData);
            Regex reg = new Regex(@"^1\|(?<mobile>\d{11})$");
            if (!string.IsNullOrEmpty(html) && reg.IsMatch(html))
                return reg.Match(html).Groups["mobile"].Value;
            return null;
        }

        public string Unlock(string mobile, string type)
        {
            string postData = string.Format("action=getMessage&sid={0}&token={1}&phone={2}&author=leodaping", type, token, mobile);
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

        public void CancelSmsRecvAll(int count)
        {
            throw new NotImplementedException();
        }

        public void AddIgnoreList(string mobile, string type)
        {
            web.Reffer = null;
            var postData = string.Format("action=addBlacklist&sid={0}&phone={1}&token={2}", type, mobile, token);
            web.Post(url, postData);
        }

        public string SendSms(string mobile, string type, string content)
        {
            web.Reffer = null;
            string postData = string.Format("action=putSentMessage&phone={0}&sid={1}&message={2}&token={3}&author=leodaping", mobile, type, content, token);//
            var html = web.Post(url, postData);
            if (html != null && html.Contains("提交成功"))
                return "succ";
            return html;
        }

        public string GetSmsStatus(string mobile, string type)
        {
            web.Reffer = null;
            string postData = string.Format("action=getSentMessageStatus&phone={0}&sid={1}&token={2}", mobile, type, token);
            var html = web.Post(url, postData);
            if (html != null && (html.Contains("发送成功") || html.Contains("手机号不在线或手机号已释放")))
                return "succ";
            return html;
        }

        public string GetSummary()
        {
            web.Reffer = null;
            string postData = string.Format("action=getSummary&token={0}", token);
            var html = web.Post(url, postData);
            if (string.IsNullOrEmpty(html))
                return "";
            //1|925.4948|7|200|软件商|false
            return Regex.Match(html, @"1\|(.*?)\|").Groups[1].Value;
        }
    }
}
