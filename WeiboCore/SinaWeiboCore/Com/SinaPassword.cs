using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SinaWeiboCore.Com
{
    /// <summary>
    /// 计算新浪密码密文
    /// </summary>
    public class SinaPassword
    {
        //static readonly string directory = Directory.GetCurrentDirectory() + "/ReffDll/";
        static readonly string directory = "";

        /// <summary>
        /// 计算新浪密码密文（登录密码加密）
        /// </summary>
        /// <param name="jeqeepassword">密码明文</param>
        /// <param name="jeqeeservertime">时间戳</param>
        /// <param name="jeqeenonce">随机值</param>
        /// <returns>密文</returns>
        public static string GetPassword(string jeqeepassword, string jeqeeservertime, string jeqeenonce)
        {
            if (!File.Exists(directory + "node.exe"))
                throw new FileNotFoundException("Nodejs程序不存在");
            if (!File.Exists(directory + "2.js"))
                throw new FileNotFoundException("登录密码加密脚本不存在");
            string cmdRunString = string.Format("{3}node 2.js {0} {1} {2}", jeqeepassword, jeqeeservertime, jeqeenonce, directory);
            return runCommand(cmdRunString);
        }

        /// <summary>
        /// 计算新浪密码密文（修改密码加密）
        /// </summary>
        /// <param name="jeqeepassword"></param>
        /// <returns></returns>
        public static string GetPassword(string jeqeepassword)
        {
            if (!File.Exists(directory + "node.exe"))
                throw new FileNotFoundException("Nodejs程序不存在");
            if (!File.Exists(directory + "1.js"))
                throw new FileNotFoundException("修改密码加密脚本不存在");
            string cmdRunString = string.Format("{1}node 1.js {0}", jeqeepassword, directory);
            return runCommand(cmdRunString);
        }

        /// <summary>
        /// 执行cmd命令
        /// </summary>
        /// <param name="cmdRunString"></param>
        /// <returns></returns>
        private static string runCommand(string cmdRunString)
        {
            Process MyProcess = new Process
            {
                StartInfo =
                {
                    //设定程序名
                    FileName = "cmd.exe",
                    //关闭Shell的使用
                    UseShellExecute = false,
                    //重定向标准输入
                    RedirectStandardInput = true,
                    //重定向标准输出
                    RedirectStandardOutput = true,
                    //重定向错误输出
                    RedirectStandardError = true,
                    //设置不显示窗口
                    CreateNoWindow = true
                }
            };

            MyProcess.Start();

            MyProcess.StandardInput.WriteLine(cmdRunString);
            //MyProcess.
            MyProcess.StandardInput.WriteLine("Exit");
            var cmd_string = MyProcess.StandardOutput.ReadToEnd();
            Regex rg = new Regex("&&&(?<pass>[0-9a-f]{256})&&&");
            //退出CMD
            if (rg.IsMatch(cmd_string))
                return rg.Match(cmd_string).Groups["pass"].Value;
            return "";
        }
    }
}
