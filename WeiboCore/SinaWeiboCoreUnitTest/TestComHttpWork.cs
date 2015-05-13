using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinaWeiboCore.Com;

namespace SinaWeiboCoreUnitTest
{
    [TestClass]
    public class TestComHttpWork
    {
        const string userName = "funny_zhoulin@163.com";
        const string password = "zhoulin";

        private static readonly object _lockObj = new object();

        /// <summary>
        /// 登陆后的Service
        /// </summary>
        private static ComWeiboLogin _currLoginUserLogin;
        private static ComWeiboLogin CurrLoginUserLogin
        {
            get
            {
                if (_currLoginUserLogin != null)
                {
                    return _currLoginUserLogin;
                }

                lock (_lockObj)
                {
                    ComWeiboLogin service = new ComWeiboLogin();

                    service.WeiboLogin(userName, password);

                    string res = string.Format("Result:{0},Error:{1},Uid:{2},Nickname:{3}", service.Result, service.Error, service.Uid, service.Nickname);
                    Console.WriteLine(res);
                    _currLoginUserLogin = service;
                    return service;
                }
            }
        }

        /// <summary>
        /// 根据uid获取用户信息
        /// </summary>
        [TestMethod]
        public void Test_GetUserEntity()
        {
            var res = ComHttpWork.Instance.GetUserEntity(CurrLoginUserLogin, "1454461772");
            Console.WriteLine(res);
        }//根据uid获取用户信息

        /// <summary>
        /// 只发布文字信息内容
        /// </summary>
        [TestMethod]
        public void Test_SendMsg()
        {
            string res = ComHttpWork.SendMsg(CurrLoginUserLogin, "今天天气晴朗_只发布文字信息内容");
            Console.WriteLine(res);
        }//只发布文字信息内容

        /// <summary>
        /// 发布文字信息与多张图片内容
        /// </summary>
        [TestMethod]
        public void Test_SendMsg_MulPic()
        {
            string res = ComHttpWork.SendMsg(CurrLoginUserLogin, new[] { "d:/p/1.jpg", "d:/p/2.jpg" }, "今天天气晴朗_多张图片内容");
            Console.WriteLine(res);
        }//发布文字信息与多张图片内容

        /// <summary>
        /// 发布文字信息与单张图片内容
        /// </summary>
        [TestMethod]
        public void Test_SendMsg_SigPic()
        {
            string res = ComHttpWork.SendMsg(CurrLoginUserLogin, "d:/p/1.jpg", "今天天气晴朗_单张图片内容");
            Console.WriteLine(res);
        }//发布文字信息与单张图片内容

        /// <summary>
        /// 发表微博
        /// </summary>
        [TestMethod]
        public void Test_AddMblog()
        {
            string res = ComHttpWork.AddMblog(CurrLoginUserLogin.Web, "d:/p/1.jpg", "今天天气晴朗_带了一张图片");
            Console.WriteLine(res);
        }//发表微博

        /// <summary>
        /// 加关注
        /// </summary>
        [TestMethod]
        public void Test_FriendCreate()
        {
            string res = ComHttpWork.FriendCreate(CurrLoginUserLogin.Web, "5171491789");
            Console.WriteLine(res);
        }//加关注

        /// <summary>
        /// 取消关注
        /// </summary>
        [TestMethod]
        public void Test_FriendDestroy()
        {
            string res = ComHttpWork.FriendDestroy(CurrLoginUserLogin.Web, "5171491789");
            Console.WriteLine(res);
        }//取消关注

        //*
        /// <summary>
        /// 点赞
        /// </summary>
        [TestMethod]
        public void Test_AttitudesCreate()
        {
        }//点赞

    }
}
