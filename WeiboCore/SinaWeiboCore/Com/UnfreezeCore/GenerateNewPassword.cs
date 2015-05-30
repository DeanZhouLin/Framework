using System.Linq;

namespace SinaWeiboCore.Com.UnfreezeCore
{
    /// <summary>
    /// 生成新密码方法
    /// </summary>
    public class GenerateNewPassword
    {
        /// <summary>
        /// 根据旧密码生成新密码 (规则最后一位不是补1；最后一位是数字、数值加1、结果10取0)
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <returns>新密码</returns>
        public static string GetNewPassword(string oldPassword)
        {
            if (oldPassword.Length > 15)
                oldPassword = oldPassword.Substring(0, 10);
            char lastChar = oldPassword.LastOrDefault();
            int tempOut;
            string newpassword;
            if (int.TryParse(lastChar.ToString(), out tempOut))
            {
                newpassword = oldPassword.Remove(oldPassword.Length - 1) + ((int.Parse(lastChar.ToString()) + 1) % 10);
            }
            else
            {
                newpassword = oldPassword + "1";
            }
            return newpassword;
        }
    }
}
