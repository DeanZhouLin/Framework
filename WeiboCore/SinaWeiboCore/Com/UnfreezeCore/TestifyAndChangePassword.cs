using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    /// <summary>
    /// 验证邮箱并更换密码
    /// </summary>
    public static class TestifyAndChangePassword
    {
        /// <summary>
        /// 验证邮箱页面
        /// </summary>
        private const string TestifyUrl = "http://login.sina.com.cn/member/testify/testify.php?page=sguide%2Fretrieve&r=http%3A%2F%2Fweibo.com&entry=weibo";

        /// <summary>
        /// 解锁新浪微博账号
        /// </summary>
        /// <param name="web">已登录新浪微博HTTP操作对象</param>
        /// <param name="email">解锁邮箱</param>
        /// <param name="oldpassword">账号原始密码</param>
        /// <returns>账号解锁成功后新密码</returns>
        public static string Run(WebAccessBase web, string email, string oldpassword)
        {
            //记录是否从手机页面转到邮箱页面
            var ischangepage = false;
            var changepageurl = "";

            var newpassword = GenerateNewPassword.GetNewPassword(oldpassword);
            web.Encode = Encoding.GetEncoding("gb2312");//操作的所有页面都使用gb2312编码
            //解锁页面
            var html = web.GetHTML(TestifyUrl);

            if (string.IsNullOrEmpty(html))
                return "网络错误，解锁失败";

            if (!html.Contains("安全邮箱"))
                return "没有安全邮箱，无法通过邮箱解锁失败";

            if (!html.Contains("确认安全邮箱"))
            {
                ischangepage = true;
                //将页面跳转至邮箱找回页面
                web.Reffer = new Uri(TestifyUrl);
                changepageurl = "http://login.sina.com.cn/member/testify/testify.php?" + GenerateChangePageData(html);
                html = web.GetHTML(changepageurl);
                //return "不是安全邮箱页面，无法通过邮箱解锁失败";
            }
            //发验证码
            var sendcodePostData = GenerateUnlockPostData(html, email);
            const string sendcodePostUrl = "http://login.sina.com.cn/member/testify/testify_sendcode.php";
            var tempHtml = web.PostWithHeaders(sendcodePostUrl, sendcodePostData, new[] { "X-Requested-With: XMLHttpRequest" });
            if (string.IsNullOrEmpty(tempHtml) || !tempHtml.Contains("100000"))
            {
                //File.AppendAllText(System.Environment.CurrentDirectory + "/解锁错误html.txt", DateTime.Now + " " + tempHtml + Environment.NewLine);
                return "页面发送邮箱验证码失败";
            }
            //接受验证码
            var door = GetDoor(email);
            if (door == "收取验证码失败")
                return "收取验证码失败";
            //验证邮箱
            var allPostData = GenerateUnlockPostData(html, email, door);
            const string allPostUrl = "http://login.sina.com.cn/member/testify/testify_all.php";
            web.Reffer = !ischangepage ? new Uri(TestifyUrl) : new Uri(changepageurl);
            tempHtml = web.PostWithHeaders(allPostUrl, allPostData, new[] { "X-Requested-With: XMLHttpRequest" });
            if (string.IsNullOrEmpty(tempHtml) || !tempHtml.Contains("100000"))
                return "验证邮箱失败";
            //GET修改密码页面
            web.Reffer = !ischangepage ? new Uri(TestifyUrl) : new Uri(changepageurl);
            web.GetHTML("http://login.sina.com.cn/member/security/password.php?entry=weibo&testified=succ");

            var ajPasswordPostData = GenerateChangePasswordPostData(oldpassword, newpassword);
            //File.AppendAllText("testifyPasswordPost.txt", ajPasswordPostData + Environment.NewLine);
            if (ajPasswordPostData == "密码数据生成失败")
                return "密码数据生成失败";

            const string ajPasswordPostUrl = "http://login.sina.com.cn/member/security/aj_password.php";
            tempHtml = web.PostWithHeaders(ajPasswordPostUrl, ajPasswordPostData, new[] { "X-Requested-With: XMLHttpRequest" });

            if (string.IsNullOrEmpty(tempHtml)) return "修改失败";
            //File.AppendAllText("testifyHtml.txt", tempHtml + Environment.NewLine);
            try
            {
                var result = DynamicJson.Parse(tempHtml);
                if (result.status == "1")
                    return newpassword;
            }
            catch
            {
            }

            return "修改失败";
        }

        /// <summary>
        /// 收取邮箱验证码
        /// </summary>
        /// <param name="email">邮箱</param>
        /// <returns>验证码(未收到:收取验证码失败)</returns>
        private static string GetDoor(string email)
        {
            var web2 = new WebAccessBase { TimeOut = 90000 };
            var door = web2.GetHTML("http://222.185.251.62:55888/getcaptchaweb2/getcaptcha.aspx?key=2EC944F05E0044E7B29F222ECBF62EC0&email=" + email);
            if (string.IsNullOrEmpty(door) || door == "未找到验证码邮件")
                door = "收取验证码失败";
            return door;
        }

        /// <summary>
        /// 生成解锁账号POST数据
        /// </summary>
        /// <param name="html">新浪解锁页面</param>
        /// <param name="email">解锁邮箱</param>
        /// <param name="door">收到的验证码(可选)</param>
        /// <returns></returns>
        private static string GenerateUnlockPostData(string html, string email, string door = "")
        {
            var paramList = GetPageData(html);
            //邮箱和验证码
            paramList.Add("answer=" + email);
            paramList.Add("door=" + door);
            //确认异常信息
            if (html.Contains("has_abnormal"))
            {
                paramList.Add("has_abnormal=1");
                paramList.Add("abnormal_validity=2");
            }
            //找回方式为邮箱
            paramList.Add("way=mail");
            paramList.Add("_t=0");
            return string.Join("&", paramList);
        }

        private static string GenerateChangePageData(string html)
        {
            var paramList = GetPageData(html);
            paramList.Add("answer=");
            paramList.Add("door=");
            if (html.Contains("has_abnormal"))
                paramList.Add("has_abnormal=1");
            //找回方式为邮箱
            paramList.Add("way=mail");
            return string.Join("&", paramList);
        }

        /// <summary>
        /// 获取页面参数
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<string> GetPageData(string html)
        {
            var regex = new Regex(@"<input type=""hidden"" name=""(?<name>.*?)"" value=""(?<value>.*?)"">");
            var matches = regex.Matches(html);
            return (from Match match in matches select match.Groups["name"].Value + "=" + match.Groups["value"].Value).ToList();
        }

        /// <summary>
        /// 生成修改密码POST数据
        /// </summary>
        /// <param name="oldpassword"></param>
        /// <param name="newpassword"></param>
        /// <returns></returns>
        private static string GenerateChangePasswordPostData(string oldpassword, string newpassword)
        {
            var paramList = new List<string>
            {
                "key=BD325CE52FC6BA090AC0C7A2039236587F99C30FA518F601F2AD33019514EE5A4340A964853E1BDF5374AB4AC22F5CFF3288E5DB94E6752B4999972DF4E23DACACAE4E4DCFB6CBAE256F1B19C4BA892D54C7A3E068F93AB47EC50635556FC223F02CB1F520631E2F03E5509B6C1E24DFB7962BCD6DC74159BF0E5AFC03D9A00D"
            };
            try
            {
                paramList.Add(string.Format("pass={0}", SinaPassword.GetPassword(oldpassword)));
                paramList.Add(string.Format("pass1={0}", SinaPassword.GetPassword(newpassword)));
                paramList.Add(string.Format("pass2={0}", SinaPassword.GetPassword(newpassword)));
            }
            catch (Exception)
            {
                return "密码数据生成失败";
            }
            return string.Join("&", paramList);
        }
    }
}
