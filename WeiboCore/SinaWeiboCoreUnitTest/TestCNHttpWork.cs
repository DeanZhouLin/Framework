using System;
using System.Collections.Generic;
using CommonEntityLib.Entities.message;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SinaWeiboCore.CN;
using CommonEntityLib.Entities.user;
using SinaWeiboCore;
using SinaWeiboCore.API;

namespace SinaWeiboCoreUnitTest
{
    [TestClass]
    public class TestCNHttpWork
    {
        const string userName = "funny_zhoulin@163.com";
        const string password = "zhoulin";

        private static readonly object _lockObj = new object();

        /// <summary>
        /// 登陆后的Service
        /// </summary>
        private static CNWeiboLogin _currWebLogin;
        private static CNWeiboLogin CurrWebLogin
        {
            get
            {
                if (_currWebLogin != null)
                {
                    return _currWebLogin;
                }

                lock (_lockObj)
                {
                    CNWeiboLogin service = new CNWeiboLogin();

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
            string res = CNHttpWork.AddMblog(CurrWebLogin.Web, "今天天气晴朗");
            Console.WriteLine(res);
        }//发微博

        /// <summary>
        /// 转发
        /// </summary>
        [TestMethod]
        public void Test_RetweetMblog()
        {
            string res = CNHttpWork.RetweetMblog(
                CurrWebLogin.Web, "http://m.weibo.cn/1454461772/C4SFndfzN",
                "C4SFndfzN".MidToId(), "呵呵 挺有意思！！！", true);
            Console.WriteLine(res);
        }//转发

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void Test_CommentMblog()
        {
            string res = CNHttpWork.CommentMblog(
                CurrWebLogin.Web, "http://m.weibo.cn/1454461772/C4SFndfzN",
                "C4SFndfzN".MidToId(), "我又来评论了", true);
            Console.WriteLine(res);
        }//评论

        /// <summary>
        /// 加关注
        /// </summary>
        [TestMethod]
        public void Test_FriendCreate()
        {
            string res = CNHttpWork.FriendCreate(CurrWebLogin.Web, "5171491789");
            Console.WriteLine(res);
        }//加关注

        /// <summary>
        /// 取消关注
        /// </summary>
        [TestMethod]
        public void Test_FriendDestroy()
        {
            string res = CNHttpWork.FriendDestroy(CurrWebLogin.Web, "5171491789");
            Console.WriteLine(res);
        }//取消关注

        /// <summary>
        /// 点赞
        /// </summary>
        [TestMethod]
        public void Test_AttitudesCreate()
        {
            string res = CNHttpWork.AttitudesCreate(CurrWebLogin.Web,
                "http://m.weibo.cn/1454461772/C4SFndfzN", "C4SFndfzN".MidToId());
            Console.WriteLine(res);
        }//点赞

        /// <summary>
        /// 取消点赞
        /// </summary>
        [TestMethod]
        public void Test_AttitudesDestroy()
        {
            string res = CNHttpWork.AttitudesDestroy(CurrWebLogin.Web,
                "http://m.weibo.cn/1454461772/C4SFndfzN", "C4SFndfzN".MidToId());
            Console.WriteLine(res);
        }//取消点赞

        /// <summary>
        /// 复制被克隆对象信息
        /// </summary>
        [TestMethod]
        public void Test_SaveInformation()
        {
            string res = CNHttpWork.SaveInformation(CurrWebLogin.Web, "5171491789", "5553089850", "deanzhou12345678");
            Console.WriteLine(res);
        }//复制被克隆对象信息

        /// <summary>
        /// 发私信
        /// </summary>
        [TestMethod]
        public void Test_SendDirectMessages()
        {
            string res = CNHttpWork.SendDirectMessages(CurrWebLogin.Web, "5553089850", "这是一封我发出的私信");
            Console.WriteLine(res);
        }//发私信

        /// <summary>
        /// 收发给自己的私信
        /// </summary>
        [TestMethod]
        public void Test_RecevieDirectMessages()
        {
            List<CnDirectMessagesEntity> res = CNHttpWork.RecevieDirectMessages(CurrWebLogin.Web, "5553089850");
            Console.WriteLine(res);
        }//收发给自己的私信

        //*
        /// <summary>
        /// 发送回复评论
        /// </summary>
        [TestMethod]
        public void Test_SendReplyComment()
        {
            //string res = CNWeiboService.SendReplyComment(CurrLoginUserService.Web,CurrLoginUserService.Uid,"C4SFndfzN",)
        }//发送回复评论

        /// <summary>
        /// 收发给自己的评论
        /// </summary>
        [TestMethod]
        public void Test_ReceiveComment()
        {
            var res = CNHttpWork.ReceiveComment(CurrWebLogin.Web, "5553089850");
            Console.WriteLine(res);
        }//收发给自己的评论

        /// <summary>
        /// 获取用户信息
        /// </summary>
        [TestMethod]
        public void Test_GetUserEntity()
        {
            var res = CNHttpWork.GetUserEntity(CurrWebLogin.Web, "5553089850");
            Console.WriteLine(res);
        }//获取用户信息

        /// <summary>
        /// 根据用户uid获取最近10条微博
        /// </summary>
        [TestMethod]
        public void Test_GetMblogs()
        {
            var res = CNHttpWork.GetMblogs(CurrWebLogin.Web, "1640057165");
            Console.WriteLine(res);
        }//根据用户uid获取最近10条微博

        /// <summary>
        /// 根据uid判断用户是否存在
        /// </summary>
        [TestMethod]
        public void Test_UserExist()
        {
            var res = CNHttpWork.UserExist(CurrWebLogin.Web, "5553089850");
            Console.WriteLine(res);
        }//根据uid判断用户是否存在

        /// <summary>
        /// 根据m.weibo.cn博文地址获取博文信息
        /// </summary>
        [TestMethod]
        public void Test_GetMblog()
        {
            CommonEntityLib.Entities.status.Entity res = CNHttpWork.GetMblog(CurrWebLogin.Web, "http://m.weibo.cn/1454461772/C4SFndfzN");
            Console.WriteLine(res);
        }//根据m.weibo.cn博文地址获取博文信息

        /// <summary>
        /// UnfreezeByAvatar
        /// </summary>
        [TestMethod]
        public void Test_UnfreezeByAvatar()
        {
            var res = CNHttpWork.UnfreezeByAvatar(CurrWebLogin.Web);
            Console.WriteLine(res);
        }//UnfreezeByAvatar [分析st失败]

        /// <summary>
        /// 获取博文列表
        /// </summary>
        [TestMethod]
        public void Test_BowenSpiderExecute()
        {
            var res = CNHttpWork.BowenSpiderExecute("1640057165");
            Console.WriteLine(res);
        }//获取博文列表 -- GetMblogs(Entity weiboUser, int page)【获取一页用户微博数据】

        /// <summary>
        /// 根据微博Url获取微博内容页面
        /// </summary>
        [TestMethod]
        public void Test_GetSingleMblog()
        {
            var res = CNHttpWork.GetSingleMblog("http://m.weibo.cn/1640057165/sd");
            Console.WriteLine(res);
        }//根据微博Url获取微博内容页面

        /// <summary>
        /// 获取最后的关注UID列表
        /// </summary>
        [TestMethod]
        public void Test_AttentionSpiderExecute1()
        {
            var res = CNHttpWork.AttentionSpiderExecute1("1640057165");
            Console.WriteLine(res);
        }//获取最后的关注UID列表 -- GetFollowers(Entity weiboUser, int page)【获取关注】 -- GetWeiboUser(string uid)【获取用户主页信息】

        /// <summary>
        /// 获取最前的关注UID列表
        /// </summary>
        [TestMethod]
        public void Test_AttentionSpiderExecute()
        {
            var res = CNHttpWork.AttentionSpiderExecute("1640057165");
            Console.WriteLine(res);
        }//获取最前的关注UID列表 -- GetFollowers(Entity weiboUser, int page)【获取关注】 -- GetWeiboUser(string uid)【获取用户主页信息】

        /// <summary>
        /// 并行获取关注的UID列表
        /// </summary>
        [TestMethod]
        public void Test_AttentionSpiderParallelExecute()
        {
            var res = CNHttpWork.AttentionSpiderParallelExecute("1640057165");
            Console.WriteLine(res);
        }//并行获取关注的UID列表 -- GetFollowers(Entity weiboUser, int page)【获取关注】 -- GetWeiboUser(string uid)【获取用户主页信息】

        /// <summary>
        /// 获取一页微博的评论数据
        /// </summary>
        [TestMethod]
        public void Test_GetComment()
        {
            var res = CNHttpWork.GetComment(1454461772, "C4SFndfzN".MidToId(), 1);
            Console.WriteLine(res);
        }//获取一页微博的评论数据

        /// <summary>
        /// 获取一页微博的评论数量
        /// </summary>
        [TestMethod]
        public void Test_GetCommentNum()
        {
            var res = CNHttpWork.GetCommentNum(1454461772, "C4SFndfzN".MidToId());
            Console.WriteLine(res);
        }//获取一页微博的评论数量

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        [TestMethod]
        public void Test_Exist()
        {
            bool res = CNHttpWork.Exist("1454461772");
            Console.WriteLine(res);
        }//检测用户是否存在

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        [TestMethod]
        public void Test_GetWeiboUserEntity()
        {
            Entity res = CNHttpWork.GetWeiboUserEntity("1454461772");
            Console.WriteLine(res);
        }//获取用户主页的信息

        /// <summary>
        /// GetWeiboHttpSearchUserIDs
        /// </summary>
        [TestMethod]
        public void Test_GetWeiboHttpSearchUserIDs()
        {
            for (int i = 1925821007; i < 1935821007; i++)
            {
                var t = ApiHttpWork.Instance.GetUserEntityColloction("2415806241", i.ToString());
                var s = ApiHttpWork.Instance.GetTags("2415806241", i.ToString());
            }
            List<string> res = CNHttpWork.GetWeiboHttpSearchUserIDs(1, 0, 0, 0, "a");
            Console.WriteLine(res);
        }//GetWeiboHttpSearchUserIDs
    }
}
