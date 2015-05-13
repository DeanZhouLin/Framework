using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CommonEntityLib.Entities;
using Jeqee.Captcha.Client;
using Jeqee.Captcha.Data;
using NLog;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.CN
{
    /// <summary>
    /// CN方式的登陆类
    /// SinaWeibo.SinaWeiboLogin
    /// </summary>
    public class CNWeiboLogin : IWeiboLogin
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly Logger CNWeiboLoginLogger = LogManager.GetLogger("CNWeiboLogin");

        /// <summary>
        /// 登陆后的结果
        /// </summary>
        public string Result { get; private set; }

        /// <summary>
        /// 运行过程中错误记录
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string Uid { get; private set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; private set; }

        /// <summary>
        /// 当前登陆的用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 当前登陆的密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 当前登陆对象的平台类型
        /// </summary>
        public PlatformType CurrPlatformType { get; private set; }

        /// <summary>
        /// Http操作对象(存储登录结果，可供后续程序使用)
        /// </summary>
        public WebAccessBase Web { get; private set; }

        /// <summary>
        /// 打码
        /// </summary>
        private readonly RuoKuaiClient _ruoKuaiClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public CNWeiboLogin()
        {
            Web = new WebAccessBase
                ("Mozilla/5.0 (iPhone; CPU iPhone OS 6_0 like Mac OS X) AppleWebKit/536.26 (KHTML, like Gecko) Version/6.0 Mobile/10A405 Safari/8536.25")
            {
                TimeOut = 60000
            };
            _ruoKuaiClient = RuoKuaiClient.CreateInstance();
            CurrPlatformType = PlatformType.CN;
            Error = "未执行";
        }

        /// <summary>
        /// 使用指定账号登录微博
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="proxy">设置代理（可选）</param>
        /// <returns>WebAccessBase：</returns>
        public void WeiboLogin(string userName, string password, string proxy = null)
        {
            try
            {
                UserName = userName;
                Password = password;
                Error = "";
                if (!string.IsNullOrEmpty(proxy))
                    Web.SetProxy(proxy, "", "");

                Web.Encode = Encoding.GetEncoding("gb2312");
                //todo m.weibo.cn
                Web.Reffer = null;
                Web.GetHTML("http://m.weibo.cn");

                const string loginUrl = "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=http%3A%2F%2Fm.weibo.cn%2F";

                Web.GetHTML(loginUrl);

                //编码用户名 @替换为%40 进行Base64编码
                string su = userName.Replace("@", "%40");
                Byte[] bufin = Encoding.UTF8.GetBytes(su);
                su = Convert.ToBase64String(bufin, 0, bufin.Length);
                var callbackStr = string.Format("jsonpcallback{0}", CommonExtension.GetTime());
                var perLoginUrl = string.Format("https://login.sina.com.cn/sso/prelogin.php?checkpin=1&entry=mweibo&su={0}&callback={1}", su, callbackStr);
                var perLoginHtml = Web.GetHTML(perLoginUrl);
                if (string.IsNullOrEmpty(perLoginHtml))
                {
                    Error = string.Format("账号:{0}密码{1}访问预处理页面失败", userName, password);
                    Result = "登录失败";
                    CNWeiboLoginLogger.Info(Error);
                    return;
                }

                CaptchaResult captchaResult = null;
                string pcid = "";
                string door = "";
                string showpin = "0";
                #region 打码
                if (perLoginHtml.Contains("\"showpin\":1"))
                {
                    showpin = "1";
                    var imageHtml = Web.GetHTML("https://passport.weibo.cn/captcha/image");
                    try
                    {
                        dynamic postResult = DynamicJson.Parse(imageHtml);
                        string retcode = postResult.retcode;
                        if (retcode != "20000000")
                        {
                            Error = string.Format("账号:{0}密码{1}获取验证码 错误代码:{2}错误信息:{3}",
                                userName, password,
                                postResult.retcode, postResult.msg);
                            Result = "获取验证码失败";
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }

                        pcid = postResult.data.pcid;
                        string imageStr = postResult.data.image;
                        imageStr = imageStr.Replace("data:image/png;base64,", "");
                        var imageByte = Convert.FromBase64String(imageStr);
                        try
                        {
                            captchaResult = _ruoKuaiClient.HandleCaptcha(imageByte, "3050", "27979", "052b72d9df844391b4cbb94b258fcb61");
                        }
                        catch (Exception ex)
                        {
                            Error = ex.ToString();
                            Result = "调用打码失败";
                            CNWeiboLoginLogger.Error(Error, ex);
                            return;
                        }

                        if (captchaResult == null)
                        {
                            Error = Result = "调用打码后结果为空";
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }

                        if (!string.IsNullOrEmpty(captchaResult.Error))
                        {
                            Error = Result = captchaResult.Error;
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }
                        door = captchaResult.CaptchaStr;
                    }
                    catch (Exception ex)
                    {
                        Error = ex.ToString();
                        Result = "执行打码失败";
                        CNWeiboLoginLogger.Error(Error, ex);
                        return;
                    }
                }
                #endregion 打码

                const string postUrl = "https://passport.weibo.cn/sso/login";
                //username=uwtsuy%40hyatt.altrality.com&password=TTDmtbgIYXY&savestate=1&ec=0&pagerefer=https%3A%2F%2Fpassport.weibo.cn%2Fsignin%2Fwelcome%3Fentry%3Dmweibo%26r%3Dhttp%253A%252F%252Fm.weibo.cn%252F%26&entry=mweibo&loginfrom=&client_id=&code=&hff=&hfp=
                string postData = string.Format(
                    "username={0}&password={1}&savestate=1{2}&ec=0&pagerefer=https%3A%2F%2Fpassport.weibo.cn%2Fsignin%2Fwelcome%3Fentry%3Dmweibo%26r%3Dhttp%253A%252F%252Fm.weibo.cn%252F%26&entry=mweibo&loginfrom=&client_id=&code=&hff=&hfp=",
                    userName.Replace("@", "%40"), password,
                    showpin == "1" ? string.Format("&pincode={0}&pcid={1}", door, pcid) : "");

                Web.Reffer = new Uri(loginUrl);
                var postHtml = Web.Post(postUrl, postData);
                if (string.IsNullOrEmpty(postHtml))
                {
                    Error = string.Format("账号:{0}密码{1}登录提交失败", userName, password);
                    Result = "登录提交失败";
                    CNWeiboLoginLogger.Info(Error);
                    return;
                }

                #region 设置cookies
                try
                {
                    dynamic postResult = DynamicJson.Parse(postHtml);
                    string retcode = postResult.retcode;
                    if (retcode == "50011010")
                    {
                        Error = string.Format("账号:{0} 密码{1} 登录提交 账号有异常 错误代码:{2} 错误信息:{3}",
                            userName, password, postResult.retcode, postResult.msg);
                        Result = "账号有异常";
                        CNWeiboLoginLogger.Info(Error);
                        return;
                    }

                    if (retcode == "50011002")
                    {
                        Error = string.Format("账号:{0} 密码{1} 登录提交 密码错误 错误代码:{2} 错误信息:{3}",
                            userName, password, postResult.retcode, postResult.msg);
                        Result = "密码错误";
                        CNWeiboLoginLogger.Info(Error);
                        return;
                    }

                    if (captchaResult != null && retcode == "50011006")
                    {
                        try
                        {
                            _ruoKuaiClient.ReportError(captchaResult.Id);
                        }
                        catch (Exception exception)
                        {
                            Debug.WriteLine(exception);
                        }
                        Error = string.Format("账号:{0}密码{1}登录提交 错误代码:{2}错误信息:{3}",
                            userName, password, postResult.retcode,
                            postResult.msg);
                        Result = "打码错误";
                        CNWeiboLoginLogger.Info(Error);
                        return;
                    }

                    if (retcode != "20000000")
                    {
                        Error = string.Format("账号:{0}密码{1}登录提交 错误代码:{2}错误信息:{3}",
                            userName, password, postResult.retcode,
                            postResult.msg);
                        Result = postResult.msg;
                        CNWeiboLoginLogger.Info(Error);
                        return;
                    }

                    Uid = postResult.data.uid;
                    if (postResult.data.IsDefined("loginresulturl") && !string.IsNullOrEmpty(postResult.data["loginresulturl"]))
                    {
                        string loginresulturl = postResult.data["loginresulturl"] + "&savestate=1&url=http%3A%2F%2Fm.weibo.cn%2F";
                        Web.Reffer = new Uri(loginUrl);
                        var temp0 = Web.GetHTML(loginresulturl);
                        if (string.IsNullOrEmpty(temp0))
                        {
                            Error = string.Format("账号{0} 密码{1} 设置weibo.cn的cookies失败", userName, password);
                            Result = "设置cookies失败";
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }
                        VerifyCnSecurityPage(userName, password);
                    }
                    else
                    {
                        string weibo_com = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["weibo.com"], CommonExtension.GetTime());
                        Web.Reffer = new Uri(loginUrl);
                        var temp1 = Web.GetHTML(weibo_com);
                        if (string.IsNullOrEmpty(temp1))
                        {
                            Error = string.Format("账号{0}密码{1}设置weibo.com的cookies失败", userName, password);
                            Result = "设置cookies失败";
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }
                        string sina_com_cn = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["sina.com.cn"], CommonExtension.GetTime());
                        Web.Reffer = new Uri(loginUrl);
                        Web.GetHTML(sina_com_cn);
                        string weibo_cn = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["weibo.cn"], CommonExtension.GetTime());
                        Web.Reffer = new Uri(loginUrl);
                        var temp2 = Web.GetHTML(weibo_cn);
                        if (string.IsNullOrEmpty(temp2))
                        {
                            Error = string.Format("账号{0}密码{1}设置weibo.cn的cookies失败", userName, password);
                            Result = "设置cookies失败";
                            CNWeiboLoginLogger.Info(Error);
                            return;
                        }
                        VerifyCnSecurityPage(userName, password);
                    }
                }
                catch (Exception ex)
                {
                    Error = string.Format("账号{0}密码{1}分析登录结果失败\r\n{2}", userName, password, postHtml);
                    Result = "分析登录结果失败";
                    CNWeiboLoginLogger.Error(Error, ex);
                }
                #endregion 设置cookies
            }
            catch (Exception ex)
            {
                Result = Error = "发生未处理异常";
                CNWeiboLoginLogger.Error(Error, ex);
            }
        }

        /// <summary>
        /// m.weibo.cn登录带设置登录保护
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="proxy"></param>
        /// <param name="protect">0代表不处理设置登录保护  1代表清空保护  2代表设置保护</param>
        /// <param name="pcList"></param>
        public void WeiboLogin(string userName, string password, string proxy, int protect, List<ProtectProvinceAndCity> pcList)
        {
            WeiboLogin(userName, password, proxy);
            if (Result == "正常")
            {
                try
                {
                    const string url1 = "http://m.weibo.cn/home/setting";
                    Web.GetHTML(url1);
                    const string url2 = "http://security.weibo.cn/";
                    Web.GetHTML(url2);
                    const string url3 = "http://security.weibo.cn/Protect/index";
                    var indexHtml = Web.GetHTML(url3);

                    const string url4 = "http://security.weibo.cn/Protect/save";
                    if (protect == 1)
                    {
                        if (!string.IsNullOrEmpty(indexHtml))
                        {
                            var v = Regex.Match(indexHtml, @"<div id=""bh_protect.*?关闭登录保护.*?</div>").Value;
                            if (string.IsNullOrEmpty(v) || !v.Contains("active"))
                            {
                                Web.Post(url4, "type=2&location=");
                            }
                        }
                    }
                    if (protect == 2 && pcList != null && pcList.Count <= 3)
                    {
                        foreach (var pc in pcList)
                        {
                            if (pc.province == "11" || pc.province == "12" || pc.province == "31" || pc.province == "50")
                            {
                                pc.city = "-1";
                            }
                        }
                        string location = DynamicJson.Serialize(pcList);
                        string postData = string.Format("type=2&location={0}", location);
                        Web.Post(url4, postData);
                    }
                }
                catch (Exception exception)
                {
                    CNWeiboLoginLogger.Error("设置登录保护", exception);
                }
            }
        }

        /// <summary>
        /// 验证登陆结果
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        private void VerifyCnSecurityPage(string username, string password)
        {
            Web.Reffer = null;
            var html = Web.GetHTML("http://m.weibo.cn/");

            if (string.IsNullOrEmpty(html))
            {
                Error = Result = "访问账号主页网络错误";
                CNWeiboLoginLogger.Info(Error);
                return;
            }

            if (html.Contains("<title>微博 - 随时随地发现新鲜事</title>") && html.Contains("{'stage':'home'}"))
            {
                if (GetUidFromCnHome(html))
                {
                    Result = "正常";
                }
                else
                {
                    Error = Result = "获取UID失败";
                    CNWeiboLoginLogger.Info(Error);
                }
                return;
            }

            //if (html.Contains("<title>帐号异常</title>") && html.Contains("免费获取验证码") && html.Contains("若绑定手机无法使用，请到互联网进行操作"))
            //{
            //    Result = "手机无法解封";
            //    return;
            //}
            if (html.Contains("<title>帐号异常</title>") && html.Contains("发送短信"))
            {
                Error = Result = "无法收短信解封";
                CNWeiboLoginLogger.Info(Error);
                return;
            }

            if (html.Contains("<title>帐号异常</title>") && html.Contains("抱歉，您的微博帐号出现异常，暂时被冻结！"))
            {
                Error = Result = "封号";
                CNWeiboLoginLogger.Info(Error);
                return;
            }

            if (html.Contains("<title>头像快速解冻 - 帐号异常</title>"))
            {
                Error = Result = "封号";
                CNWeiboLoginLogger.Info(Error);
                return;
            }

            if (html.Contains("<title>帐号异常</title>") && html.Contains("在线申诉"))
            {
                Error = Result = "死号";
                CNWeiboLoginLogger.Info(Error);
                return;
            }

            Error = string.Format("未知失败 账号{0}密码{1}登录主页分析失败\r\n{2}", username, password, html);
            Result = "未知失败";
            CNWeiboLoginLogger.Info(Error);
        }

        /// <summary>
        /// 从页面获取UID
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private bool GetUidFromCnHome(string html)
        {
            Regex rg_nickname = new Regex("\"userName\":\"(?<nickname>.*?)\"");
            Nickname = rg_nickname.Match(html).Groups["nickname"].Value;
            Nickname = Nickname.NormalU2C();
            Regex rg_uid = new Regex("\"uid\":\"(?<uid>.*?)\"");
            Uid = rg_uid.Match(html).Groups["uid"].Value;
            return !string.IsNullOrEmpty(Uid);// && !string.IsNullOrEmpty(Nickname);
        }
    }
}
