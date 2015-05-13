using System;
using SinaWeiboCore.CN;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    /// <summary>
    /// 处理账号异常
    /// </summary>
    public class AccountExceptionHandling
    {
        //private static readonly Logger Logger = LogManager.GetLogger("解封");

        public Object[] jiefeng2(string username, string password, string receive_sms_name, string receive_sms_account, string receive_sms_password, string send_sms_name, string send_sms_account, string send_sms_password, string type)
        {
            string result = "";
            ComWeiboLogin login = new ComWeiboLogin();
            if (login.Web == null)
                result = "SinaVisitorSystem失败";
            try
            {
                login.WeiboLogin(username, password);
                string loginResult = login.Result;
                switch (loginResult)
                {
                    case "正常":
                        result = "正常";
                        break;
                    case "锁定":
                        result = "锁定";
                        break;
                    case "死号":
                        result = "死号";
                        break;
                    case "无法收短信解封":
                        {
                            if (type == "water")
                                result = loginResult;
                            else if (type == "small_weibo")
                            {
                                Unfreeze unfreeze = new Unfreeze(send_sms_name, send_sms_account, send_sms_password);
                                result = unfreeze.Run(login.Web);
                            }
                            break;
                        }
                    case "封号":
                        {
                            Unfreeze unfreeze = new Unfreeze(receive_sms_name, receive_sms_account, receive_sms_password);
                            result = unfreeze.Run(login.Web);
                            break;
                        }
                    case "密码错误":
                        result = "密码错误";
                        break;
                    default:
                        result = loginResult;
                        break;
                }
            }
            catch (Exception err)
            {
                result = err.Message;
                //File.AppendAllText("jiefengerr.txt", DateTime.Now + "\t" + err + "\t" + username + "\t" + password + Environment.NewLine);
            }
            return new Object[] { login, result };
        }

        public string jiesuo(string username, string password, string email)
        {
            ComWeiboLogin login = new ComWeiboLogin();
            login.WeiboLogin(username, password);
            string loginResult = login.Result;
            string result;
            switch (loginResult)
            {
                case "正常":
                    result = "正常";
                    break;
                case "锁定":
                    if (string.IsNullOrEmpty(email))
                    {
                        result = "解锁失败email为空";
                        break;
                    }
                    var unlockResult = TestifyAndChangePassword.Run(login.Web, email, password);
                    if (unlockResult.Contains("失败"))
                        result = unlockResult;
                    else
                        result = "解锁成功" + unlockResult;
                    break;
                case "死号":
                    result = "死号";
                    break;
                case "封号":
                    result = "封号";
                    break;
                case "密码错误":
                    result = "密码错误";
                    break;
                default:
                    result = loginResult;
                    break;
            }
            return result;
        }

        public Object[] jiefeng(string username, string password, string receive_sms_name, string receive_sms_account,
            string receive_sms_password, string send_sms_name, string send_sms_account, string send_sms_password,
            string type)
        {
            string result = "";
            IWeiboLogin login = new CNWeiboLogin();
            if (login.Web == null)
                result = "SinaVisitorSystem失败";
            try
            {
                login.WeiboLogin(username, password);
                string loginResult = login.Result;
                switch (loginResult)
                {
                    case "正常":
                        result = "正常";
                        break;
                    case "锁定":
                        result = "锁定";
                        break;
                    case "死号":
                        result = "死号";
                        break;
                    case "无法收短信解封":
                        {
                            //Logger.Info("账号:{0}密码:{1},无法收短信解封", username, password);
                            if (type == "water")
                                result = loginResult;
                            else if (type == "small_weibo")
                            {
                                //login.Web.UserAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.1; WOW64; Trident/6.0)";
                                Unfreeze unfreeze = new Unfreeze(send_sms_name, send_sms_account, send_sms_password);
                                login = new ComWeiboLogin();
                                login.WeiboLogin(username, password);
                                result = unfreeze.Run(login.Web);
                            }
                            break;
                        }
                    case "封号":
                        {
                            result = CNHttpWork.UnfreezeByAvatar(login.Web);
                            if (result == "分析st失败")
                            {
                                Unfreeze unfreeze = new Unfreeze(receive_sms_name, receive_sms_account, receive_sms_password);
                                login = new ComWeiboLogin();
                                login.WeiboLogin(username, password);
                                result = unfreeze.Run(login.Web);
                            }
                            break;
                        }
                    case "密码错误":
                        result = "密码错误";
                        break;
                    default:
                        result = loginResult;
                        break;
                }
            }
            catch (Exception err)
            {
                result = err.Message;
                //File.AppendAllText("jiefengerr.txt", DateTime.Now + "\t" + err + "\t" + username + "\t" + password + Environment.NewLine);
            }
            return new Object[] { login, result };
        }
    }
}
