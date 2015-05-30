using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CommonEntityLib.Entities;
using Jeqee.Captcha.Client;
using Jeqee.Captcha.Data;
using NLog;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.Com
{
    /// <summary>
    /// Com方式的登陆类
    /// SinaWeibo.SinaWeiboLogin
    /// </summary>
    public class ComWeiboLogin : IWeiboLogin
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly Logger ComWeiboLoginLogger = LogManager.GetLogger("ComWeiboService");

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
        /// 当前登陆对象的平台类型
        /// </summary>
        public PlatformType CurrPlatformType { get; private set; }

        /// <summary>
        /// Http操作对象(存储登录结果，可供后续程序使用)
        /// </summary>
        public WebAccessBase Web { get; private set; }

        /// <summary>
        /// 当前登陆的用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 当前登陆的密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 打码
        /// </summary>
        private readonly RuoKuaiClient _ruoKuaiClient;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComWeiboLogin()
        {
            Web = new WebAccessBase { TimeOut = 60000 };
            _ruoKuaiClient = RuoKuaiClient.CreateInstance();
            CurrPlatformType = PlatformType.Com;
            Error = "未执行";
        }

        /// <summary>
        /// 使用指定账号登录微博[COM]
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="proxy">设置代理（可选）</param>
        /// <returns>WebAccessBase：Http操作对象(存储登录结果，可供后续程序使用)</returns>
        public void WeiboLogin(string userName, string password, string proxy = null)
        {
            try
            {
                UserName = userName;
                Password = password;
                if (!string.IsNullOrEmpty(proxy))
                    Web.SetProxy(proxy, "", "");

                Web.Encode = Encoding.GetEncoding("gb2312");
                //存储访问网页后获取的源码信息
                //访问weibo主页
                var html = Web.GetHTML("http://weibo.com");
                if (string.IsNullOrEmpty(html))
                {
                    Result = "GET新浪微博主页网络错误";
                    Error = string.Format("{0} {1}", Result, Web.Error.Message);
                    ComWeiboLoginLogger.Error(Error, Web.Error);
                    return;
                }

                #region 获取登录时的校验信息

                //编码用户名 @替换为%40 进行Base64编码
                string su = userName.Replace("@", "%40");
                Byte[] bufin = Encoding.UTF8.GetBytes(su);
                su = Convert.ToBase64String(bufin, 0, bufin.Length);
                //拼接地址
                string preLoginPageUrl = string.Format("http://login.sina.com.cn/sso/prelogin.php?entry=weibo&callback=sinaSSOController.preloginCallBack&su={0}&rsakt=mod&checkpin=1&client=ssologin.js(v1.4.15)&_={1}", su, CommonExtension.GetTime().ToString(CultureInfo.InvariantCulture));
                //获取校验信息
                html = Web.GetHTML(preLoginPageUrl);
                if (string.IsNullOrEmpty(html))
                {
                    Result = "GET新浪微博prelogin页面网络错误";
                    Error = string.Format("{0} {1}", Result, Web.Error.Message);
                    ComWeiboLoginLogger.Error(Error, Web.Error);
                    return;
                }

                #endregion 获取登录时的校验信息

                #region 检查并提取校验信息

                string retcode = "";
                string servertime;
                string nonce;
                string rsakv;
                string showpin;
                string pcid;
                string door = "";
                CaptchaResult captchaResult = null;
                try
                {
                    Regex rg_preloginCallBack = new Regex(@"sinaSSOController.preloginCallBack\((?<json>.*?)\)");
                    var json_preloginCallBack = rg_preloginCallBack.Match(html).Groups["json"].Value;
                    dynamic preInfo = DynamicJson.Parse(json_preloginCallBack);
                    retcode = preInfo.retcode;
                    servertime = preInfo.servertime;
                    nonce = preInfo.nonce;
                    rsakv = preInfo.rsakv;
                    showpin = preInfo.showpin;
                    pcid = preInfo.pcid;
                }
                catch (Exception ex)
                {
                    Result = "解析prelogin结果出错";
                    Error = string.Format("{0} {1}", Result, retcode);
                    ComWeiboLoginLogger.Error(Error, ex);
                    return;
                }

                #region 打码

                if (showpin == "1")
                {
                    string picurl = string.Format("http://login.sina.com.cn/cgi/pin.php?r={1}&s=0&p={0}", pcid, new Random(Guid.NewGuid().GetHashCode()).Next(10000000, 99999999));
                    Web.Reffer = new Uri("http://weibo.com/");
                    var imageByte = Web.GetImageByte(picurl);
                    try
                    {
                        //File.WriteAllBytes("pic.jpg", imageByte);
                        //captchaResult = CaptchaClient.Client.HandleCaptcha(imageByte);
                        captchaResult = _ruoKuaiClient.HandleCaptcha(imageByte, "3050", "27979", "052b72d9df844391b4cbb94b258fcb61");
                    }
                    catch (Exception ex)
                    {
                        Result = "调用打码失败";
                        Error = string.Format("{0} {1}", Result, ex.Message);
                        ComWeiboLoginLogger.Error(Error, ex);
                        return;
                    }
                    if (captchaResult == null)
                    {
                        Error = Result = "调用打码后结果为空";
                        ComWeiboLoginLogger.Info(Error);
                        return;
                    }
                    if (!string.IsNullOrEmpty(captchaResult.Error))
                    {
                        Error = Result = captchaResult.Error;
                        ComWeiboLoginLogger.Info(Error);
                        return;
                    }
                    door = captchaResult.CaptchaStr;
                }

                #endregion 打码

                #endregion 检查并提取校验信息

                #region 生成登录信息

                string sp = "";
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        sp = SinaPassword.GetPassword(password, servertime, nonce);
                        if (!string.IsNullOrEmpty(sp))
                        {
                            Result = "";
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Error = Result = "计算密码密文出错" + ex;
                    }
                }
                if (string.IsNullOrEmpty(sp))
                {
                    Result = "计算密码密文失败";
                    ComWeiboLoginLogger.Error(Error);
                    return;
                }
                Error = "";
                int prelt = new Random(Guid.NewGuid().GetHashCode()).Next(50, 500);
                //entry=weibo&gateway=1&from=&savestate=0&useticket=1&pagerefer=&vsnf=1&su=enBtaW5keWUlNDBqb2xseS5mYW50YXN5YXBwbGUuY29t&service=miniblog&servertime=1370330588&nonce=GPDB7O&pwencode=rsa2&rsakv=1330428213&sp=51f8c7f774ff71f31e5b181df5adfadff257b4d3299ddae8f7e6c3b7656fd9604ac8796973ff314d934984b2fdd1df4636dc52a3d2ba6d575758da929c36df9761beb1820a9509a9cd35e9c467a206efab379b10803a98f626a28baec918cf35e7f1f10c463a86abdd45619a28c579088802172e30bc4ac8d4c93d24ebd7a60e&encoding=UTF-8&prelt=73&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META
                string postData = string.Format("entry=weibo&gateway=1&from=&savestate=0&useticket=1&pagerefer={6}&vsnf=1&su={0}&service=miniblog&servertime={1}&nonce={2}&pwencode=rsa2&rsakv={3}&sp={4}&sr=1440*900&encoding=UTF-8&prelt={5}&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META",
                    su, servertime, nonce, rsakv, sp, prelt, showpin == "1" ? string.Format("&pcid={0}&door={1}", pcid, door) : "");

                #endregion 生成登录信息

                #region 登录新浪

                Web.Reffer = new Uri("http://weibo.com/");
                //登录的POST地址
                string loginPostUrl = "http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)";
                html = Web.Post(loginPostUrl, postData);
                if (string.IsNullOrEmpty(html))
                {
                    Result = "POST新浪微博login页面网络错误";
                    Error = string.Format("{0} {1}", Result, Web.Error.Message);
                    ComWeiboLoginLogger.Error(Error, Web.Error);
                    return;
                }
                if (html.Contains("retcode=101"))
                {
                    Error = Result = "密码错误";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }
                if (html.Contains("retcode=4040"))
                {
                    Error = Result = "登录次数过多";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }
                if (html.Contains("retcode=4057"))
                {
                    Error = Result = "账号有异常";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }
                if (html.Contains("retcode=4069"))
                {
                    Error = Result = "账号未激活";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }
                if (!html.Contains("retcode=0") && !html.Contains("retcode=4049"))
                {
                    Error = string.Format("账号:{0}密码{1}登录提交失败\r\n{2}", userName, password, html);
                    Result = "登录失败";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }

                #endregion 登录新浪

                #region 获取并访问设置Cookies地址

                Regex regex = new Regex(@"location.replace\([""'](?<url>.*?)[""']\);");
                string weiboCookieUrl = regex.Match(html).Groups["url"].Value;
                if (string.IsNullOrEmpty(weiboCookieUrl))
                {
                    Error = Result = "登录结果中未匹配到ajax地址";
                    ComWeiboLoginLogger.Info(Error);
                    return;
                }
                string temp1 = Web.GetHTML(weiboCookieUrl);
                if (string.IsNullOrEmpty(temp1))
                {
                    Result = "GET新浪微博ajaxlogin页面网络错误";
                    Error = string.Format("{0} {1}", Result, Web.Error.Message);
                    ComWeiboLoginLogger.Error(Error, Web.Error);
                    return;
                }
                dynamic loginResult;
                if (temp1.Contains("<title>Sina Visitor System</title>"))
                {
                    temp1 = "{\"result\":false,\"errno\":\"4049\",\"reason\":\"\"}";
                    loginResult = DynamicJson.Parse(temp1);
                }
                else
                {
                    temp1 = temp1.Replace("<html><head><script language='javascript'>parent.sinaSSOController.feedBackUrlCallBack({", "{").Replace("});</script></head><body></body></html>", "}");

                    try
                    {
                        loginResult = DynamicJson.Parse(temp1);
                    }
                    catch (Exception ex)
                    {
                        Result = "反序列化出错 temp1=[" + temp1 + "]";
                        Error = string.Format("{0} {1}", Result, ex.Message);
                        ComWeiboLoginLogger.Error(Error, ex);
                        return;
                    }
                }

                string weiboUrl = "http://weibo.com/";
                if (loginResult.result)
                {
                    weiboUrl += loginResult.userinfo.userdomain;
                }
                else
                {
                    string errno = loginResult.errno;
                    switch (errno)
                    {
                        case "5":
                            Error = Result = "用户名错误";
                            return;

                        case "101":
                            Error = Result = "密码错误";
                            return;

                        case "2070":
                            try
                            {
                                if (captchaResult != null) _ruoKuaiClient.ReportError(captchaResult.Id);
                            }
                            catch (Exception)
                            {
                                // ignored
                            }
                            Error = Result = "验证码错误";
                            return;

                        case "4040":
                            Error = Result = "登录次数过于频繁";
                            return;

                        case "4049":
                            {
                                #region 打码再登陆一次

                                string picurl = string.Format("http://login.sina.com.cn/cgi/pin.php?r={1}&s=0&p={0}", pcid,
                                    new Random(Guid.NewGuid().GetHashCode()).Next(10000000, 99999999));
                                Web.Reffer = new Uri("http://weibo.com/");
                                var imageByte = Web.GetImageByte(picurl);
                                try
                                {
                                    //File.WriteAllBytes("pic.jpg", imageByte);
                                    //captchaResult = CaptchaClient.Client.HandleCaptcha(imageByte);
                                    captchaResult = _ruoKuaiClient.HandleCaptcha(imageByte, "3050", "27979", "052b72d9df844391b4cbb94b258fcb61");
                                }
                                catch (Exception ex)
                                {
                                    Result = "调用打码失败";
                                    Error = ex.Message;
                                    return;
                                }
                                if (captchaResult == null)
                                {
                                    Result = "调用打码后结果为空";
                                    return;
                                }
                                if (!string.IsNullOrEmpty(captchaResult.Error))
                                {
                                    Result = captchaResult.Error;
                                    return;
                                }
                                door = captchaResult.CaptchaStr;

                                try
                                {
                                    sp = SinaPassword.GetPassword(password, servertime, nonce);
                                }
                                catch (Exception ex)
                                {
                                    Result = "计算密码密文出错";
                                    Error = ex.Message;
                                    return;
                                }
                                if (string.IsNullOrEmpty(sp))
                                {
                                    Result = "计算密码密文失败";
                                    return;
                                }
                                prelt = new Random(Guid.NewGuid().GetHashCode()).Next(50, 500);
                                postData =
                                    string.Format(
                                        "entry=weibo&gateway=1&from=&savestate=0&useticket=1&pagerefer={6}&vsnf=1&su={0}&service=miniblog&servertime={1}&nonce={2}&pwencode=rsa2&rsakv={3}&sp={4}&sr=1440*900&encoding=UTF-8&prelt={5}&url=http%3A%2F%2Fweibo.com%2Fajaxlogin.php%3Fframelogin%3D1%26callback%3Dparent.sinaSSOController.feedBackUrlCallBack&returntype=META",
                                        su, servertime, nonce, rsakv, sp, prelt, string.Format("&pcid={0}&door={1}", pcid, door));

                                Web.Reffer = new Uri("http://weibo.com/");
                                //登录的POST地址
                                loginPostUrl = "http://login.sina.com.cn/sso/login.php?client=ssologin.js(v1.4.15)";
                                html = Web.Post(loginPostUrl, postData);
                                if (string.IsNullOrEmpty(html))
                                {
                                    Result = "POST新浪微博login页面网络错误";
                                    Error = Web.Error.Message;
                                    return;
                                }

                                if (html.Contains("retcode=101"))
                                {
                                    Result = "密码错误";
                                    return;
                                }
                                if (html.Contains("retcode=4040"))
                                {
                                    Result = "登录次数过多";
                                    return;
                                }
                                if (html.Contains("retcode=4057"))
                                {
                                    Result = "账号有异常";
                                    return;
                                }
                                if (html.Contains("retcode=2070"))
                                {
                                    try
                                    {
                                        _ruoKuaiClient.ReportError(captchaResult.Id);
                                    }
                                    catch (Exception)
                                    {
                                        // ignored
                                    }
                                    Result = "验证码错误";
                                    return;
                                }
                                if (html.Contains("retcode=4069"))
                                {
                                    Result = "账号未激活";
                                    return;
                                }
                                if (!html.Contains("retcode=0"))
                                {
                                    ComWeiboLoginLogger.Info("账号:{0}密码{1}登录提交失败\r\n{2}", userName, password, html);
                                    Result = "登录失败2";
                                    return;
                                }

                                regex = new Regex(@"location.replace\([""'](?<url>.*?)[""']\);");
                                weiboCookieUrl = regex.Match(html).Groups["url"].Value;
                                if (string.IsNullOrEmpty(weiboCookieUrl))
                                {
                                    Result = "登录结果中未匹配到ajax地址";
                                    return;
                                }
                                temp1 = Web.GetHTML(weiboCookieUrl);
                                if (string.IsNullOrEmpty(temp1))
                                {
                                    Result = "GET新浪微博ajaxlogin页面网络错误";
                                    Error = Web.Error.Message;
                                    return;
                                }

                                temp1 =
                                    temp1.Replace(
                                        "<html><head><script language='javascript'>parent.sinaSSOController.feedBackUrlCallBack({",
                                        "{").Replace("});</script></head><body></body></html>", "}");
                                loginResult = DynamicJson.Parse(temp1);
                                weiboUrl = "http://weibo.com/";
                                if (loginResult.result)
                                {
                                    weiboUrl += loginResult.userinfo.userdomain;
                                }
                                else
                                {
                                    errno = loginResult.errno;
                                    switch (errno)
                                    {
                                        case "5":
                                            Result = "用户名错误";
                                            return;

                                        case "101":
                                            Result = "密码错误";
                                            return;

                                        case "2070":
                                            try
                                            {
                                                _ruoKuaiClient.ReportError(captchaResult.Id);
                                            }
                                            catch (Exception)
                                            {
                                                // ignored
                                            }
                                            Result = "验证码错误";
                                            return;

                                        case "4040":
                                            Result = "登录次数过于频繁";
                                            return;

                                        case "4057":
                                            Result = "账号有异常";
                                            return;

                                        default:
                                            Result = loginResult.reason;
                                            return;
                                    }
                                }

                                #endregion 打码再登陆一次
                            }
                            break;

                        case "4057":
                            Error = Result = "账号有异常";
                            return;

                        case "4069":
                            Error = Result = "账号未激活";
                            return;

                        default:
                            Result = loginResult.reason;
                            return;
                    }
                }
                Web.GetHTML(weiboUrl);

                #endregion 获取并访问设置Cookies地址

                #region 访问微博设置页面判断账号状态

                html = Web.GetHTML("http://weibo.com/");
                if (!string.IsNullOrEmpty(html) && html.Contains("您当前使用的账号存在异常，请完成以下操作解除异常状态"))
                {
                    Error = Result = "无法收短信解封";
                    return;
                }
                if (!string.IsNullOrEmpty(html) && html.Contains("<title>微博帐号解冻"))
                {
                    Error = Result = "封号";
                    return;
                }
                if (!string.IsNullOrEmpty(html) && html.Contains("<title>404错误") && html.Contains("在线申诉"))
                {
                    Error = Result = "死号";
                    return;
                }
                html = Web.GetHTML("http://security.weibo.com/security/index");
                //判断账号状态
                if (!string.IsNullOrEmpty(html))
                {
                    if ((html.Contains("帐号安全系统检测到您的帐号存在高危风险")))
                    {
                        Error = Result = "锁定";
                        return;
                    }
                    if (html.Contains("修改密码"))
                    {
                        Result = GetUidFromHTML(html) ? "正常" : "获取UID失败";
                    }
                    else
                    {
                        //File.AppendAllText("SinaWeiboLoginUnknownError.txt", userName + "\t" + password + Environment.NewLine);
                        ComWeiboLoginLogger.Info("账号{0}登录后安全页面分析失败\r\n{1}", userName, html);
                        Error = Result = "未知失败";
                    }
                }
                else
                {
                    Error = Result = "GET新浪微博账号安全页面网络错误";
                }

                #endregion 访问微博设置页面判断账号状态
            }
            catch (Exception ex)
            {
                Error = Result = "发生未处理异常";
                ComWeiboLoginLogger.Error(Error, ex);
            }
        }

        public void WeiboLogin(string userName, string password, string proxy, int protect, List<ProtectProvinceAndCity> pcList)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 从账号设置页面获取账号昵称和Uid
        /// </summary>
        /// <param name="html">账号设置页面</param>
        private bool GetUidFromHTML(string html)
        {
            Regex rg_nickname = new Regex(@"\$CONFIG.nickname = '(?<nickname>.*?)';");
            Nickname = rg_nickname.Match(html).Groups["nickname"].Value;
            Regex rg_uid = new Regex(@"\$CONFIG.uid = '(?<uid>.*?)';");
            Uid = rg_uid.Match(html).Groups["uid"].Value;
            return !string.IsNullOrEmpty(Uid);// && !string.IsNullOrEmpty(Nickname);
        }
    }
}
