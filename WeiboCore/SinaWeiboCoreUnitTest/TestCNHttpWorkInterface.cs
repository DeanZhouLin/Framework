using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinaWeiboCore;
using SinaWeiboCore.CN;

namespace SinaWeiboCoreUnitTest
{
    [TestClass]
    public class TestCNHttpWorkInterface
    {
        const string userName = "funny_zhoulin@163.com";
        const string password = "zhoulin";

        private static readonly object _lockObj = new object();

        private const PlatformType CurrPlatformType = PlatformType.CN;

        /// <summary>
        /// 登陆后的Service
        /// </summary>
        private static IWeiboLogin _currWebLogin;
        private static IWeiboLogin CurrWebLogin
        {
            get
            {
                if (_currWebLogin != null)
                {
                    return _currWebLogin;
                }

                lock (_lockObj)
                {
                    IWeiboLogin service = CurrPlatformType.GetWeiboLogin();

                    service.WeiboLogin(userName, password);
                    string res = string.Format("Result:{0},Error:{1},Uid:{2},Nickname:{3}", service.Result, service.Error, service.Uid, service.Nickname);
                    Console.WriteLine(res);
                    _currWebLogin = service;
                    return service;
                }
            }
        }

        /// <summary>
        /// 发微博
        /// </summary>
        [TestMethod]
        public void Test_AddMblog()
        {
            string res = CurrPlatformType.GetHttpWork().
                AddMblog(CurrWebLogin, "我发图片玩123", "",
                "D:\\p\\a.jpg",
                "D:\\p\\b.jpg",
                "D:\\p\\c.jpg",
                "D:\\p\\d.jpg",
                "D:\\p\\e.jpg",
                "D:\\p\\f.gif",
                "D:\\p\\g.png");
            Console.WriteLine(res);
        }//发微博

        /// <summary>
        /// 添加图片
        /// </summary>
        [TestMethod]
        public void Test_AddPic()
        {
            List<string> res = CurrPlatformType.GetHttpWork().
                AddPicture(
                CurrWebLogin,
                @"D:\p\f.gif",
                @"C:\Users\Public\Pictures\Sample Pictures\Hydrangeas.jpg");

            string t = string.Join(" ", res);
            Console.WriteLine(t);
        }//发微博

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        [TestMethod]
        public void Test_GetWeiboUserEntity()
        {
            var res = CurrPlatformType.GetHttpWork().GetUserEntity(CurrWebLogin, CurrWebLogin.Uid);
            Console.WriteLine(res);
        }//获取用户主页的信息
    }
}
