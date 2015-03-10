using JFx.Utils;
using System;
using System.Net;

namespace JFx
{
    /// <summary>
    /// 获取当前应用程序环境信息
    /// </summary>
    public static class AppEnvironment
    {
        /// <summary>
        /// 获取此本地计算机的 NetBIOS 名称。
        /// </summary>
        public static string MachineName
        {
            get { return Environment.MachineName; }
        }
        /// <summary>
        /// 获取与当前用户关联的网络域名。
        /// </summary>
        public static string UserDomainName
        {
            get { return Environment.UserDomainName; }
        }
        /// <summary>
        /// 获取当前已登录到 Windows 操作系统的人员的用户名。
        /// </summary>
        public static string UserName
        {
            get { return Environment.UserName; }
        }
        /// <summary>
        /// 获取包含当前平台标识符和版本号的 System.OperatingSystem 对象
        /// </summary>
        public static string OSVersion
        {
            get { return Environment.OSVersion.ToString(); }
        }
        /// <summary>
        /// 获取当前计算机上的处理器数。
        /// </summary>
        public static string CPUCount
        {
            get { return Environment.ProcessorCount.ToString(); }
        }

        /// <summary>
        /// 获取该进程的命令行。
        /// </summary>
        public static string CommandLine
        {
            get { return Environment.CommandLine; }
        }

        /// <summary>
        /// 本地计算机的主机名
        /// </summary>
        public static string HostName
        {
            get { return Utility.InvokeFuncWithCatch<string>(() => Dns.GetHostName()); }
        }

        private static string localIPAddress = "0.0.0.0";

        /// <summary>
        /// 本机IP地址，非客户端IP，默认：string.Empty
        /// </summary>
        public static string LocalIPAddress
        {
            get
            {
                if (localIPAddress == "0.0.0.0")
                {
                    localIPAddress = Utility.InvokeFuncWithCatch<string>(() => Utility.GetLocalIPAddress(), string.Empty);
                }
                return localIPAddress;
            }
        }

        private static int appId = int.MinValue;
        /// <summary>
        /// 获取当前应用程序编号
        /// <para>默认：0</para>
        /// <para>获取方式：ConfigurationManager.AppSettings["AppId"]</para>
        /// </summary>
        public static int AppId
        {
            get
            {
                if (appId == int.MinValue)
                {
                    appId = System.Configuration.ConfigurationManager.AppSettings["AppId"].ConvertTo<int>(0, true);
                }
                return appId;
            }
        }
    }
}
