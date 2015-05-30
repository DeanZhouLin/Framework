using System;
using System.Text.RegularExpressions;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    public class Yunma : ISmsapi
    {
        private readonly WebAccessBase _web;
        private const string BaseUrl = "http://api.vim6.com/DevApi/";
        private readonly string _uid;
        private readonly string _token;

        public string Balance;
        public string Result { get; set; }

        public Yunma(string username, string password)
        {
            _web = new WebAccessBase { TimeOut = 60000 };
            const string url = BaseUrl + "loginIn";
            string postData = string.Format("uid={0}&pwd={1}", username, password);
            string html = _web.Post(url, postData);
            if (string.IsNullOrEmpty(html) || !html.Contains("Token"))
            {
                throw new Exception("云码初始化失败");
            }
            try
            {
                var json = DynamicJson.Parse(html);
                _uid = json.Uid;
                _token = json.Token;
                Balance = json.Balance;
            }
            catch (Exception)
            {
                throw new Exception("云码初始化失败");
            }
        }

        public string GetMobile(string type)
        {
            //type 40 新浪微博解锁
            const string url = BaseUrl + "getMobilenum";
            string postData = string.Format("uid={0}&pid={1}&token={2}&size=1", _uid, type, _token);
            string html = _web.Post(url, postData);
            if (!string.IsNullOrEmpty(html) && Regex.IsMatch(html, @"\d{11}"))
            {
                return html;
            }
            return null;
        }

        public string Unlock(string mobile, string type)
        {
            const string url = BaseUrl + "getVcodeAndReleaseMobile";
            //mobile=用getMobilenum方法获取到的手机号&token=用loginIn方法获取到的token&uid=登录返回的Uid&author_uid=开发者用户名&pid=项目ID
            string postData = string.Format("mobile={0}&token={1}&uid={2}&author_uid=leodaping&pid={3}", mobile, _token, _uid, type);
            string html = _web.Post(url, postData);
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
            const string url = BaseUrl + "addIgnoreList";
            //pid=项目ID&mobiles=以,号分隔的手机号列表&uid=登录返回的Uid&token=登录时返回的令牌
            string postData = string.Format("pid={3}&mobiles={0}&token={1}&uid={2}", mobile, _token, _uid, type);
            Result = _web.Post(url, postData);
            const string url2 = BaseUrl + "cancelSMSRecv";
            //mobile=用getMobilenum方法获取到的手机号&uid=登录返回的Uid&token=用loginIn方法获取到的token
            string postData2 = string.Format("mobile={0}&token={1}&uid={2}", mobile, _token, _uid);
            Result = _web.Post(url2, postData2);
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
