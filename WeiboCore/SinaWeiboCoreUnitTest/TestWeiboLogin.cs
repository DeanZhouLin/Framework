using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinaWeiboCore;

namespace SinaWeiboCoreUnitTest
{
    [TestClass]
    public class TestWeiboLogin
    {
        /// <summary>
        /// Com登陆测试
        /// </summary>
        [TestMethod]
        public void ComLoginTest()
        {
            ComLoginTest("funny_zhoulin@163.com", "zhoulin");
        }//Com登陆测试

        /// <summary>
        /// CN登陆测试
        /// </summary>
        [TestMethod]
        public void CNLoginTest()
        {
            CNLoginTest("funny_zhoulin@163.com", "zhoulin");
        }//CN登陆测试

        /// <summary>
        /// Com登陆测试
        /// </summary>
        /// <param name="un"></param>
        /// <param name="pd"></param>
        private static void ComLoginTest(string un, string pd)
        {
            IWeiboLogin service = PlatformType.Com.GetWeiboLogin();

            service.WeiboLogin(un, pd);

            string res = string.Format("Result:{0},Error:{1},Uid:{2},Nickname:{3}", service.Result, service.Error, service.Uid, service.Nickname);
            Console.WriteLine(res);
        }

        /// <summary>
        /// CN登陆测试
        /// </summary>
        /// <param name="un"></param>
        /// <param name="pd"></param>
        private static void CNLoginTest(string un, string pd)
        {
            IWeiboLogin service = PlatformType.CN.GetWeiboLogin();

            service.WeiboLogin(un, pd);

            string res = string.Format("Result:{0},Error:{1},Uid:{2},Nickname:{3}", service.Result, service.Error, service.Uid, service.Nickname);
            Console.WriteLine(res);
        }

        /// <summary>
        /// 过期账号登陆测试
        /// </summary>
        [TestMethod]
        public void GUOQILoginTest()
        {
            const string userName = "wuglwg@handy.remember-jobs.com";
            const string password = "2hlqLWzVmr";

            ComLoginTest(userName, password);
            CNLoginTest(userName, password);
        }//过期账号登陆测试

        /// <summary>
        /// 异常账号登陆测试
        /// </summary>
        [TestMethod]
        public void YCLoginTest()
        {
            const string userName = "impnwv@mackey.liningsneaker.com";
            const string password = "kwpOqqBc1jt1";

            ComLoginTest(userName, password);
            CNLoginTest(userName, password);
        }//异常账号登陆测试

        /// <summary>
        /// 密码错误登陆测试
        /// </summary>
        [TestMethod]
        public void MMCWLoginTest()
        {
            const string userName = "18612793094";
            const string password = "wmm131421";

            ComLoginTest(userName, password);
            CNLoginTest(userName, password);
        }//密码错误登陆测试

        /// <summary>
        /// 账号锁定登陆测试
        /// </summary>
        [TestMethod]
        public void ZHSDLoginTest()
        {
            const string userName = "1071989848@qq.com";
            const string password = "qazwsx1990";

            ComLoginTest(userName, password);
            CNLoginTest(userName, password);
        }//账号锁定登陆测试
    }
}
