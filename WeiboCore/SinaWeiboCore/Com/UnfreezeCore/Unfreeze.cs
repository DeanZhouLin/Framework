using System;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    public class Unfreeze
    {
        readonly ISmsapi smsapi;

        //微博收短信
        private readonly string weibo_receive_type;
        //微博发短信
        private readonly string weibo_send_type = "";
        public string platform;

        public Unfreeze(string sms_name, string sms_account, string sms_password)
        {
            platform = sms_name;
            switch (sms_name)
            {
                case "飞Q":
                    smsapi = new Xudan123(sms_account, sms_password);
                    weibo_receive_type = "48";
                    //_type2 = "641";
                    break;
                case "F02":
                    smsapi = new F02(sms_account, sms_password);
                    weibo_send_type = "8421";
                    weibo_receive_type = "123";
                    //_type2 = "1454";
                    break;
                case "JikeSMS":
                    smsapi = new JikeSms(sms_account, sms_password);
                    weibo_receive_type = "6";
                    //_type2 = "69";
                    break;
                case "Taoma":
                    smsapi = new Taoma(sms_account, sms_password);
                    weibo_receive_type = "32";
                    break;
                case "Yzm1":
                    smsapi = new Yzm1(sms_account, sms_password);
                    weibo_receive_type = "1000";
                    weibo_send_type = "1003";
                    break;
                case "Yunma":
                    smsapi = new Yunma(sms_account, sms_password);
                    weibo_receive_type = "40";
                    break;
                default:
                    throw new Exception("短信平台未能匹配");
            }
            //_smsapi.CancelSmsRecvAll();
        }

        //weibo.com解封
        public string Run(WebAccessBase web)
        {
            try
            {
                web.Reffer = null;
                string html1 = web.GetHTML("http://weibo.com/");
                if (!string.IsNullOrEmpty(html1) && html1.Contains("您当前使用的账号存在异常，请完成以下操作解除异常状态"))
                {
                    #region 发短信解封
                    for (int m = 0; m < 10; m++)
                    {
                        string mobile = "";
                        try
                        {
                            mobile = smsapi.GetMobile(weibo_send_type);
                            if (string.IsNullOrEmpty(mobile))
                            {
                                //File.AppendAllText("mobile获取为空.txt", DateTime.Now + Environment.NewLine);
                                Thread.Sleep(2000);
                                continue;
                            }
                            //提交手机号到新浪
                            var url = "http://sass.weibo.com/aj/upside/nextstep?__rnd=" + CommonExtension.GetTime();
                            var postData = string.Format("mobile={0}&zone=0086&_t=0", mobile);
                            web.Reffer = new Uri("http://sass.weibo.com/unfreeze");
                            var html = web.PostWithHeaders(url, postData, new[] { "X-Requested-With: XMLHttpRequest" });
                            if (string.IsNullOrEmpty(html))
                                continue;
                            var temp1 = DynamicJson.Parse(html);
                            if (temp1.code == "1000")
                            {
                                //发短信
                                html = smsapi.SendSms(mobile, weibo_send_type, "26");
                                if (html != "succ")
                                {
                                    //File.AppendAllText("SendSms失败.txt", DateTime.Now + "\t" + html + "\t" + mobile + Environment.NewLine);
                                    //发生失败后，拉黑手机号，重做
                                    //_smsapi.AddIgnoreList(mobile, _send_type);
                                    //Thread.Sleep(1000);
                                    continue;
                                }
                                // 收发送结果
                                string result = "";
                                int i1 = 0;
                                while (i1 < 10)
                                {
                                    result = smsapi.GetSmsStatus(mobile, weibo_send_type);
                                    if (result == "succ")
                                        break;
                                    Thread.Sleep(3000);
                                    i1++;
                                }
                                if (i1 >= 10)
                                {
                                    //File.AppendAllText("一码收短信超时.txt", DateTime.Now + "\t" + mobile + Environment.NewLine);
                                    continue;
                                }
                                if (result == "succ")
                                {
                                    //这里要循环检查
                                    for (int i = 0; i < 10; i++)
                                    {
                                        Thread.Sleep(1000 * 6);
                                        url = "http://sass.weibo.com/aj/upside/check?__rnd=" + CommonExtension.GetTime();
                                        postData = string.Format("mobile={0}&_t=0", mobile);
                                        web.Reffer = new Uri("http://sass.weibo.com/unfreeze");
                                        html = web.PostWithHeaders(url, postData, new[] { "X-Requested-With: XMLHttpRequest" });
                                        if (html == "{\"code\":\"1000\"}")
                                        {
                                            //成功处理
                                            web.Reffer = new Uri("http://sass.weibo.com/unfreeze");
                                            web.GetHTML("http://sass.weibo.com/unfreeze?step=2");
                                            return "发短信解封成功";
                                        }
                                    }
                                    //File.AppendAllText("发短信后检查超时.txt", DateTime.Now + "\t" + html + "\t" + mobile + Environment.NewLine);
                                    return "发短信后检查超时";
                                }
                            }
                            string message = temp1.msg;
                            if (message.Contains("您的账号验证过于频繁") || message.Contains("系统繁忙，请稍候再试吧"))
                                return message;
                            if (message.Contains("换个号码吧"))
                            {
                                Thread.Sleep(3000);
                            }
                        }
                        catch (RuntimeBinderException)
                        {
                        }
                        catch (Exception err)
                        {
                            return err.Message;
                            //File.AppendAllText("UnfreezeRunErr.txt", err + Environment.NewLine);
                        }
                        finally
                        {
                            if (!string.IsNullOrEmpty(mobile))
                                smsapi.AddIgnoreList(mobile, weibo_send_type);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region 收短信解封
                    for (int i = 0; i < 30; i++)
                    {
                        string mobile = "";
                        try
                        {
                            mobile = smsapi.GetMobile(weibo_receive_type);
                            if (mobile == "获取手机号失败")
                                return "获取手机号失败";
                            string url1 = "http://sass.weibo.com/aj/mobile/unfreeze?__rnd=" + CommonExtension.GetTime();
                            string post1 = string.Format("value={0}&zone=0086&_t=0", mobile);
                            web.Reffer = new Uri("http://sass.weibo.com/aj/quickdefreeze/mobile");
                            html1 = web.PostWithHeaders(url1, post1, new[] { "x-requested-with: XMLHttpRequest" });
                            if (string.IsNullOrEmpty(html1))
                                continue;
                            var objectError = DynamicJson.Parse(html1);
                            string message = objectError.msg;
                            if (message.Contains("您的账号验证过于频繁") || message.Contains("系统繁忙，请稍候再试吧"))
                                return message;
                            else if (message.Contains("输入的手机号码"))
                                continue;
                            else
                            {
                                int count = 0;//循环计数
                                string verified = "";
                                while (verified.Length != 6)//收激活码
                                {
                                    if (count > 10 || verified.Contains("获取验证码失败"))//工作1分钟后退出
                                        break;
                                    Thread.Sleep(5000);
                                    verified = smsapi.Unlock(mobile, weibo_receive_type);
                                    count++;
                                }
                                if (verified.Length == 6)
                                {
                                    string url2 = "http://sass.weibo.com/aj/user/checkstatus?__rnd=" + CommonExtension.GetTime();
                                    string post2 = string.Format("code={0}&_t=0", verified);
                                    web.Reffer = new Uri("http://sass.weibo.com/aj/quickdefreeze/mobile");
                                    string html2 = web.PostWithHeaders(url2, post2, new string[] { "x-requested-with: XMLHttpRequest" });
                                    if (!string.IsNullOrEmpty(html2) && html2.Contains("100000"))
                                        return "解封成功";
                                    else
                                    {
                                        //File.AppendAllText("解封失败.txt", "收短信" + html2 + "\t" + mobile + "\t" + verified + Environment.NewLine);
                                        return "解封失败";
                                    }
                                }
                            }
                        }
                        catch (RuntimeBinderException)
                        {
                        }
                        catch (Exception err)
                        {
                            return err.Message;
                            //File.AppendAllText("UnfreezeRunErr.txt", err + Environment.NewLine);
                        }
                        finally
                        {
                            if (!string.IsNullOrEmpty(mobile))
                                smsapi.AddIgnoreList(mobile, weibo_receive_type);
                        }
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return "解封超时";
        }

        //weibo.cn解封
        [Obsolete]
        public string Run3(string username, string password)
        {
            ////var web = new WebAccessBase("Mozilla/5.0 (Linux; Android 4.1.1; Nexus 7 Build/JRO03D) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.166  Safari/535.19")
            ////{
            ////    TimeOut = 60000
            ////};
            //var web = new WebAccessBase { TimeOut = 60000 };
            //#region 访问主页
            //string html = web.GetHTML("http://weibo.cn/");
            //if (string.IsNullOrEmpty(html) || !html.Contains("<title>微博广场</title>"))
            //    return "访问CN主页失败";
            //#endregion
            //#region 访问登录页
            //var urlLogin = Regex.Match(html, "<a href='(?<urlLogin>.*?)'>登录</a>").Groups["urlLogin"].Value;
            //if (string.IsNullOrEmpty(urlLogin))
            //    return "获取登录URL失败";
            //urlLogin = urlLogin.Replace("&amp;", "&");
            //html = web.GetHTML(urlLogin);
            //if (string.IsNullOrEmpty(html) || !html.Contains("<title>微博</title>"))
            //    return "访问登录页面失败";
            //#endregion
            //#region 登录

            //try
            //{
            //    var urlLoginPost =
            //        Regex.Match(html, @"<form action=""(?<urlLoginPost>.*?)"" method=""post"">").Groups["urlLoginPost"]
            //            .Value;
            //    urlLoginPost = urlLoginPost.Replace("&amp;", "&");
            //    urlLoginPost = "https://login.weibo.cn/login/" + urlLoginPost;
            //    var postList = new List<string>
            //    {
            //        string.Format("mobile={0}", Uri.EscapeDataString(username))
            //    };
            //    var passwordName =
            //        Regex.Match(html, @"<input type=""password"" name=""(?<passwordName>.*?)""").Groups["passwordName"]
            //            .Value;
            //    postList.Add(string.Format("{0}={1}", passwordName, password));
            //    postList.AddRange(
            //        from Match match in
            //            Regex.Matches(html, @"input type=""hidden"" name=""(?<name>.*?)"" value=""(?<value>.*?)""")
            //        select
            //            string.Format("{0}={1}", match.Groups["name"].Value,
            //                Uri.EscapeDataString(match.Groups["value"].Value)));
            //    postList.Add("submit=%E7%99%BB%E5%BD%95");
            //    var dataLoginPost = string.Join("&", postList);
            //    html = web.Post(urlLoginPost, dataLoginPost);
            //}
            //catch (Exception exception)
            //{
            //    Debug.WriteLine(exception);
            //}

            //#endregion
            //if (string.IsNullOrEmpty(html) || html.Contains("你当前使用的帐号存在异常，请完成手机验证，提升帐号安全"))
            //    return "无法收短信解封";
            //if (string.IsNullOrEmpty(html) || !html.Contains("您的微博帐号出现异常，暂时被冻结"))
            //    return "登录后未到达解封页面";

            //#region 解封
            //for (int i = 0; i < 30; i++)
            //{
            //    string mobile = "获取手机号失败";
            //    try
            //    {
            //        mobile = GetMobile();
            //        if (mobile == "获取手机号失败")
            //            return "获取手机号失败";
            //        var ac = Regex.Match(html, @"<input type=""hidden"" name=""ac""\s*?value=""(.*?)""/>").Groups[1].Value;
            //        string getMessagePostData = string.Format("ac={0}&region=0086&ztype=CN&action=unbind&phone={1}&wm=&from=", ac, mobile);
            //        string getMessagePostUrl = "http://m.weibo.cn/security/sendVerifyCode";
            //        web.Reffer = new Uri("http://m.weibo.cn/security?vt=4");
            //        html = web.Post(getMessagePostUrl, getMessagePostData);
            //        if (string.IsNullOrEmpty(html))
            //        {
            //            _smsapi.AddIgnoreList(mobile, _type);
            //            continue;
            //        }
            //        var objectError = DynamicJson.Parse(html);
            //        string message = objectError.msg;
            //        if (message.Contains("您的账号验证过于频繁"))
            //            return message;
            //        else if (message.Contains("一个手机号不能验证多个账号"))
            //            continue;
            //        else
            //        {
            //            int count = 0;//循环计数
            //            string verified = "";
            //            while (verified.Length != 6)//收激活码
            //            {
            //                if (count > 10 || verified.Contains("获取验证码失败"))//工作1分钟后退出
            //                    break;
            //                Thread.Sleep(5000);
            //                verified = _smsapi.Unlock(mobile, _type);
            //                count++;
            //            }
            //            if (verified.Length == 6)
            //            {
            //                string verifyUrl = "http://m.weibo.cn/security/unfreeUser";
            //                string verifyPostData = string.Format("ac={0}&phone={1}&ztype=&code={2}&wm=&from=", ac, mobile, verified);
            //                web.Reffer = new Uri("http://m.weibo.cn/security?vt=4");
            //                html = web.Post(verifyUrl, verifyPostData);
            //                if (!string.IsNullOrEmpty(html) && html.Contains("\"ok\":1"))
            //                    return "解封成功";
            //                else
            //                {
            //                    File.AppendAllText("解封失败.txt", html + "\t" + mobile + "\t" + verified + Environment.NewLine);
            //                    return "解封失败";
            //                }
            //            }
            //        }
            //    }
            //    catch (Exception err)
            //    {
            //        File.AppendAllText("err.txt", err + Environment.NewLine);
            //    }
            //    finally
            //    {
            //        _smsapi.AddIgnoreList(mobile, _type);
            //    }
            //}
            //#endregion

            return "解封超时";
        }

        [Obsolete]
        public string Run2(string username, string password)
        {
            WebAccessBase web = new WebAccessBase();
            try
            {
                //web.Encode = Encoding.GetEncoding("gb2312");
                //web.Reffer = new Uri("http://weibo.com/");
                //var html = web.GetHTML("http://login.sina.com.cn/member/testify/testify.php?entry=weibo");
                //if (!string.IsNullOrEmpty(html) && html.Contains("填写登录帐号"))
                //{
                //    var regex = new Regex(@"<input type=""hidden"".*?name=""(.*?)"" value=""(.*?)"">");
                //    var temp = regex.Matches(html)
                //        .Cast<Match>()
                //        .Aggregate("",
                //            (current, match) => current + (match.Groups[1].Value + "=" + match.Groups[2].Value + "&"));
                //    temp += "username=" + username + "&";
                //    temp += "password=" + SinaPassword.GetPassword(password);
                //    html = web.Post("http://login.sina.com.cn/member/testify/testify2.php", temp);

                //    if (!string.IsNullOrEmpty(html) && html.Contains("填写手机号验证帐号"))
                //    {
                //        for (int i = 0; i < 30; i++)
                //        {
                //            //取手机号
                //            string mobile = GetMobile(_type2);
                //            if (mobile == "获取手机号失败")
                //                return "获取手机号失败";
                //            temp = regex.Matches(html)
                //                .Cast<Match>()
                //                .Aggregate("",
                //                    (current, match) =>
                //                        current + (match.Groups[1].Value + "=" + match.Groups[2].Value + "&"));

                //            temp += "scellphone=" + mobile + "&entry=weibo&loginname=" + username.Replace("@", "%40");
                //            var url = "http://login.sina.com.cn/member/testify/testify3_sms.php?rnd=" +
                //                      new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                //            html = web.Post(url, temp);
                //            if (!string.IsNullOrEmpty(html) && html.Contains("A00006"))
                //            {
                //                #region 收短信

                //                string rencode = "";
                //                int count = 0;
                //                while (rencode.Length != 6) //收激活码
                //                {
                //                    if (count > 10) //工作1分钟后退出
                //                    {
                //                        //SMSApi.SMSApi.cancelSMSRecvAll(sms_username, token);
                //                        //_smsapi.CancelSmsRecvAll();
                //                        _smsapi.AddIgnoreList(mobile);
                //                        return "接受手机验证码超时" + rencode;
                //                    }
                //                    Thread.Sleep(5000);
                //                    rencode = _smsapi.Unlock(mobile, _type2); //爱码是123,飞Q是48
                //                    //if (rencode.Equals("mobile_state_error") || rencode.Equals("parameter_error"))
                //                    //{
                //                    //    //_smsapi.CancelSmsRecvAll();
                //                    //    _smsapi.AddIgnoreList(mobile);
                //                    //    return "接受手机验证码错误" + rencode;
                //                    //}
                //                    //if (verified.Length > 6 && !verified.Equals("not_receive"))
                //                    //if (rencode.Length == 6)
                //                    //    File.AppendAllText("VerifiedLog.txt",
                //                    //        DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t手机号\t" + mobile + "\t" +
                //                    //        rencode + Environment.NewLine);
                //                    count++;
                //                }

                //                #endregion

                //                temp = temp.Replace("scellphone=" + mobile, "rencode=" + rencode);

                //                url = "http://login.sina.com.cn/member/testify/testify4.php?rnd=" +
                //                      new Random(Guid.NewGuid().GetHashCode()).NextDouble();
                //                web.Reffer = new Uri("http://login.sina.com.cn/member/testify/testify2.php");
                //                html = web.Post(url, temp);
                //                if (!string.IsNullOrEmpty(html) && html.Contains("A00006"))
                //                {
                //                    //var result = DynamicJson.Parse(html);
                //                    //url = result.data.url;
                //                    //html = web.GetHTML(url);
                //                    return "解封成功";
                //                }
                //            }
                //            else
                //            {
                //                _smsapi.AddIgnoreList(mobile, _type2);
                //                //_smsapi.CancelSmsRecvAll();
                //            }
                //        }
                //        return "账号解封但经过多次尝试没有可用手机号";
                //    }
                //    return "账号解封但未进入填写手机号验证帐号页面";
                //}
                return "账号解封但未进入填写登录帐号页面";
            }
            catch (Exception)
            {
                return "解封异常";
            }
        }
    }
}
