using System;
using System.Collections.Generic;
using CommonEntityLib.Entities;
using CommonEntityLib.Entities.message;
using CommonEntityLib.Entities.tag;
using CommonEntityLib.Entities.user;
using SinaWeiboCore.CN;
using SinaWeiboCore.Com;

namespace SinaWeiboCore
{
    /// <summary>
    /// 平台类型
    /// </summary>
    public enum PlatformType
    {
        /// <summary>
        /// CN
        /// </summary>
        CN,

        /// <summary>
        /// Com
        /// </summary>
        Com,

        /// <summary>
        /// Api
        /// </summary>
        Api
    }

    /// <summary>
    /// 登陆对象接口
    /// </summary>
    public interface IWeiboLogin
    {
        /// <summary>
        /// 登陆结果
        /// </summary>
        string Result { get; }

        /// <summary>
        /// 登陆过程中的错误信息
        /// </summary>
        string Error { get; }

        /// <summary>
        /// 用户ID
        /// </summary>
        string Uid { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        string Nickname { get; }

        /// <summary>
        /// 当前登陆用户名
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 当前登陆密码
        /// </summary>
        string Password { get; }

        /// <summary>
        /// 当前登陆对象的平台类型
        /// </summary>
        PlatformType CurrPlatformType { get; }

        /// <summary>
        /// Http操作对象(存储登录结果，可供后续程序使用)
        /// </summary>
        WebAccessBase Web { get; }

        /// <summary>
        /// 使用指定账号登录微博
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="proxy">设置代理（可选）</param>
        /// <returns>WebAccessBase：</returns>
        void WeiboLogin(string userName, string password, string proxy = null);

        /// <summary>
        /// m.weibo.cn登录带设置登录保护
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="proxy"></param>
        /// <param name="protect">0代表不处理设置登录保护  1代表清空保护  2代表设置保护</param>
        /// <param name="pcList"></param>
        void WeiboLogin(string userName, string password, string proxy, int protect, List<ProtectProvinceAndCity> pcList);
    }

    /// <summary>
    /// HttpWork对象接口
    /// </summary>
    public interface IHttpWork
    {
        /// <summary>
        /// 发微博
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="text">微博内容 不能超过140字</param>
        /// <param name="appkey">appkey 可以为null</param>
        /// <param name="picPidOrPaths">图片Pid集合、路径集合</param>
        /// <returns>结果 空字符串表示成功</returns>
        string AddMblog(IWeiboLogin webLogin, string text, string appkey, params string[] picPidOrPaths);//发微博

        /// <summary>
        /// 批量上传图片到新浪
        /// </summary>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="picContents">图片Byte[]集合</param>
        /// <returns>图片Pid列表</returns>
        List<string> AddPicture(IWeiboLogin webLogin, params byte[][] picContents);//批量上传图片到新浪

        /// <summary>
        /// 批量上传图片到新浪
        /// </summary>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="picPaths">图片路径集合</param>
        /// <returns>图片Pid列表</returns>
        List<string> AddPicture(IWeiboLogin webLogin, params string[] picPaths);//批量上传图片到新浪

        /// <summary>
        /// 加关注
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">关注用户uid</param>
        /// <returns>结果 空字符串表示成功</returns>
        string FriendCreate(IWeiboLogin webLogin, string uid);//加关注

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">取消关注用户uid</param>
        /// <returns>结果 空字符串表示成功</returns>
        string FriendDestroy(IWeiboLogin webLogin, string uid);//取消关注

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="weiboUrl">微博url</param>
        /// <param name="weiboId">微博id</param>
        /// <returns>结果 空字符串表示成功</returns>
        string AttitudesCreate(IWeiboLogin webLogin, string weiboUrl, string weiboId);//点赞

        /// <summary>
        /// 删除微博
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="url"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        string DelMblog(IWeiboLogin webLogin, string url, string mid);//删除微博

        /// <summary>
        /// 用户信息搜索
        /// </summary>
        /// <param name="factor1">类型：只搜公司 1 只搜学校 2 只搜标签 3</param>
        /// <param name="factor2">性别：男性 4  女性 5</param>
        /// <param name="factor3">用户类型：机构认证用户 6 个人认证用户 7 普通用户 8</param>
        /// <param name="factor4">年龄段：18以下_9 19到22_10 23到29_11 30到39_12 40以上_13</param>
        /// <param name="queryWord">查询字符串</param>
        /// <returns>符合条件的UserID列表</returns>
        List<string> GetWeiboHttpSearchUserIDs(int factor1, int factor2, int factor3, int factor4, string queryWord);//用户信息搜索

        //*Com未实现这个方法
        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="cloneUid"></param>
        /// <returns></returns>
        string UploadAvatar(IWeiboLogin webLogin, string cloneUid);

        //*Com未实现这个方法
        /// <summary>
        /// 取消赞
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="weiboUrl">微博url</param>
        /// <param name="weiboId">微博id</param>
        /// <returns>结果 空字符串表示成功</returns>
        string AttitudesDestroy(IWeiboLogin webLogin, string weiboUrl, string weiboId);//取消赞

        //*Com未实现这个方法
        /// <summary>
        /// 复制被克隆对象信息
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="targetUid"></param>
        /// <param name="uid">小号的uid</param>
        /// <param name="nickname">小号待更新的昵称</param>
        /// <returns>结果 空字符串表示成功</returns>
        string SaveInformation(IWeiboLogin webLogin, string targetUid, string uid, string nickname);//复制被克隆对象信息

        //*Com未实现这个方法
        /// <summary>
        /// 发私信
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">私信对象</param>
        /// <param name="text">私信内容</param>
        /// <returns>结果 空字符串表示成功</returns>
        string SendDirectMessages(IWeiboLogin webLogin, string uid, string text);//发私信

        //*Com未实现这个方法
        /// <summary>
        /// 收发给自己的私信
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">账号自己的Uid</param>
        /// <returns>私信列表</returns>
        List<CnDirectMessagesEntity> RecevieDirectMessages(IWeiboLogin webLogin, string uid);//收发给自己的私信

        //*Com未实现这个方法
        /// <summary>
        /// 收所有的私信会话
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<CnDirectMessagesEntity> RecevieAllDirectMessages(IWeiboLogin webLogin, string uid);//收所有的私信会话

        //*Com未实现这个方法
        /// <summary>
        /// 发送回复评论
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">工作号自己的Uid（用于记录日志）</param>
        /// <param name="mid">评论的那条微博mid</param>
        /// <param name="cid">被回复的评论cid</param>
        /// <param name="replyName">被回复人的昵称</param>
        /// <param name="replyContent">回复评论内容</param>
        /// <returns>结果 空字符串表示成功</returns>
        string SendReplyComment(IWeiboLogin webLogin, string uid, string mid,
            string cid, string replyName, string replyContent);//发送回复评论

        //*Com未实现这个方法
        /// <summary>
        /// 收发给自己的评论
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<CommonEntityLib.Entities.comment.Entity> ReceiveComment(IWeiboLogin webLogin, string uid);//收发给自己的评论

        /// <summary>
        /// 根据uid获取用户信息
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        Entity GetUserEntity(IWeiboLogin webLogin, string uid);//根据uid获取用户信息

        //*Com未实现这个方法
        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Entity GetUserEntity(string uid);//获取用户主页的信息

        //*仅Api实现
        /// <summary>
        /// 批量获取用户信息
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        Collection GetUserEntityColloction(string appKey, string uids);//批量获取用户信息

        //*仅Api实现
        /// <summary>
        /// 批量获取用户Tag信息
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        Dictionary<string, IEnumerable<Tag>> GetTags(string appKey, string uids);//批量获取用户Tag信息

        //*Com未实现这个方法
        /// <summary>
        /// 根据用户uid获取最近10条微博
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<CommonEntityLib.Entities.status.Entity> GetMblogs(IWeiboLogin webLogin, string uid);//根据用户uid获取最近10条微博

        //*Com未实现这个方法
        /// <summary>
        /// 根据UID获取第一页博文列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<CommonEntityLib.Entities.status.Entity> GetMblogs(string uid);//根据UID获取第一页博文列表

        //*Com未实现这个方法
        /// <summary>
        /// 获取指定页码的微博
        /// 遇到指定的mid则不再查找
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">被查找的uid</param>
        /// <param name="mid">小于该指定的mid不再查找</param>
        /// <param name="page">查找页数</param>
        /// <returns></returns>
        List<CommonEntityLib.Entities.status.Entity> GetMblogs(WebAccessBase web, string uid, string mid, int page);//获取指定页码的微博

        //*Com未实现这个方法
        /// <summary>
        /// 根据uid判断用户是否存在
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool UserExist(IWeiboLogin webLogin, string uid);//根据uid判断用户是否存在

        //*Com未实现这个方法
        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        bool UserExist(string uid);//检测用户是否存在

        //*Com未实现这个方法
        /// <summary>
        /// 获取新增粉丝的第一页
        /// </summary>
        /// <param name="webLogin"></param>
        List<string> GetNewFans(IWeiboLogin webLogin);//获取新增粉丝的第一页

        //*Com未实现这个方法
        /// <summary>
        /// 根据m.weibo.cn博文地址获取博文信息
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        CommonEntityLib.Entities.status.Entity GetMblog(IWeiboLogin webLogin, string url);// 根据m.weibo.cn博文地址获取博文信息

        //*Com未实现这个方法
        /// <summary>
        /// 获取最后的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>最后的关注UID列表</returns>
        List<long> GetLastAttentionUIDs(string uid);//获取最后的关注UID列表。

        //*Com未实现这个方法
        /// <summary>
        /// 获取最前的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>获取最前的关注UID列表</returns>
        List<long> GetPrewAttentionUIDs(string uid);//获取最前的关注UID列表。

        //*Com未实现这个方法
        /// <summary>
        /// 获取20个fans
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>20个fans</returns>
        List<long> Get20Fans(string uid);// 获取20个fans

        //*Com未实现这个方法
        /// <summary>
        /// 并行获取关注的UID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        List<long> GetAttentionUIDsAsParallel(string uid);//并行获取关注的UID列表

    }

    /// <summary>
    /// 对象获取工厂类
    /// </summary>
    public static class HttpWorkFactory
    {
        /// <summary>
        /// 根据平台类型获取登陆对象
        /// </summary>
        /// <param name="platformType">平台类型：CN Com</param>
        /// <returns>对应平台的登陆新浪微博对象</returns>
        public static IWeiboLogin GetWeiboLogin(this PlatformType platformType)
        {
            switch (platformType)
            {
                case PlatformType.CN:
                    return new CNWeiboLogin();
                case PlatformType.Com:
                    return new ComWeiboLogin();
                default:
                    return null;
            }
        }

        /// <summary>
        /// 根据平台类型获取HttpWork对象
        /// </summary>
        /// <param name="platformType">平台类型：CN Com</param>
        /// <returns>对应平台的HttpWork对象</returns>
        public static IHttpWork GetHttpWork(this PlatformType platformType)
        {
            switch (platformType)
            {
                case PlatformType.CN:
                    return CNHttpWork.Instance;
                case PlatformType.Com:
                    return ComHttpWork.Instance;
                case PlatformType.Api:
                    return ComHttpWork.Instance;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 登陆对象向下转换
        /// </summary>
        /// <typeparam name="T">具体登陆平台对象类型</typeparam>
        /// <param name="webLogin">抽象对象</param>
        /// <returns>具体登陆平台对象</returns>
        internal static T GetWeiboLogin<T>(this IWeiboLogin webLogin) where T : class
        {
            T login = webLogin as T;
            if (login == null)
            {
                throw new Exception("登陆对象为空");
            }
            return login;
        }
    }
}
