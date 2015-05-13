using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using CommonEntityLib.Entities.comment;
using CommonEntityLib.Entities.message;
using CommonEntityLib.Entities.tag;
using NLog;
using SinaWeiboCore.ReffDll;
using Collection = CommonEntityLib.Entities.user.Collection;

namespace SinaWeiboCore.CN
{
    /// <summary>
    /// CN方式的HttpWork类
    /// </summary>
    public class CNHttpWork : IHttpWork
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly Logger CNHttpWorkLogger = LogManager.GetLogger("CNHttpWork");

        /// <summary>
        /// CNHttpWork实例
        /// </summary>
        public static readonly CNHttpWork Instance = new CNHttpWork();

        /// <summary>
        /// 构造函数
        /// </summary>
        private CNHttpWork()
        {

        }

        /// <summary>
        /// 发微博
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="text">微博内容 不能超过140字</param>
        /// <param name="appkey">appkey 可以为null</param>
        /// <param name="picPidOrPaths">图片Pid集合、路径集合</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string AddMblog(IWeiboLogin webLogin, string text, string appkey, params string[] picPidOrPaths)
        {
            text.CheckLength(280);
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();

            //获取pid
            string picsPid = picPidOrPaths.GetPicsPid(webLogin, ",");

            //发带图片微博
            return AddMblog(login.Web, text, picsPid, appkey);
        }

        /// <summary>
        /// 批量上传图片到新浪
        /// </summary>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="picContents">图片Byte[]集合</param>
        /// <returns>图片Pid列表</returns>
        public List<string> AddPicture(IWeiboLogin webLogin, params byte[][] picContents)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            List<string> res = new List<string>();
            foreach (byte[] pic in picContents)
            {
                string pichtml = login.Web.UploadSinaWeiboImage(pic, Guid.NewGuid().ToString(), "http://m.weibo.cn/mblogDeal/addpic");
                if (!string.IsNullOrEmpty(pichtml))
                {
                    var pObj = DynamicJson.Parse(pichtml);
                    try
                    {
                        if (pObj.ok == "1")
                        {
                            res.Add(pObj.pic_id);
                        }
                    }
                    catch (Exception ex)
                    {
                        CNHttpWorkLogger.Error("上传图片异常", ex);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 批量上传图片到新浪
        /// </summary>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="picPaths">图片路径集合</param>
        /// <returns>图片Pid列表</returns>
        public List<string> AddPicture(IWeiboLogin webLogin, params string[] picPaths)
        {
            if (picPaths == null)
            {
                throw new Exception("图片地址不能为空");
            }
            return AddPicture(webLogin, picPaths.Select(File.ReadAllBytes).ToArray());
        }

        /// <summary>
        /// 加关注
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">关注用户uid</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string FriendCreate(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return FriendCreate(login.Web, uid);
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">取消关注用户uid</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string FriendDestroy(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return FriendDestroy(login.Web, uid);
        }

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="weiboUrl">微博url</param>
        /// <param name="weiboId">微博id</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string AttitudesCreate(IWeiboLogin webLogin, string weiboUrl, string weiboId)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return AttitudesCreate(login.Web, weiboUrl, weiboId);
        }

        /// <summary>
        /// 删除微博
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="url"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        public string DelMblog(IWeiboLogin webLogin, string url, string mid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return DelMblog(login.Web, url, mid);
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="cloneUid"></param>
        /// <returns></returns>
        public string UploadAvatar(IWeiboLogin webLogin, string cloneUid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return UploadAvatar(login.Web, cloneUid);
        }

        /// <summary>
        /// 取消赞
        /// Com未实现这个方法
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="weiboUrl">微博url</param>
        /// <param name="weiboId">微博id</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string AttitudesDestroy(IWeiboLogin webLogin, string weiboUrl, string weiboId)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return AttitudesDestroy(login.Web, weiboUrl, weiboId);
        }

        /// <summary>
        /// 复制被克隆对象信息
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="targetUid"></param>
        /// <param name="uid">小号的uid</param>
        /// <param name="nickname">小号待更新的昵称</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string SaveInformation(IWeiboLogin webLogin, string targetUid, string uid, string nickname)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return SaveInformation(login.Web, targetUid, uid, nickname);
        }

        /// <summary>
        /// 发私信
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">私信对象</param>
        /// <param name="text">私信内容</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string SendDirectMessages(IWeiboLogin webLogin, string uid, string text)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return SendDirectMessages(login.Web, uid, text);
        }

        /// <summary>
        /// 收发给自己的私信
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">账号自己的Uid</param>
        /// <returns>私信列表</returns>
        public List<CnDirectMessagesEntity> RecevieDirectMessages(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return RecevieDirectMessages(login.Web, uid);
        }

        /// <summary>
        /// 收所有的私信会话
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CnDirectMessagesEntity> RecevieAllDirectMessages(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return RecevieAllDirectMessages(login.Web, uid);
        }

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
        public string SendReplyComment(IWeiboLogin webLogin, string uid, string mid, string cid, string replyName, string replyContent)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return SendReplyComment(login.Web, uid, mid, cid, replyName, replyContent);
        }

        /// <summary>
        /// 收发给自己的评论
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<Entity> ReceiveComment(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return ReceiveComment(login.Web, uid);
        }

        /// <summary>
        /// 根据uid获取用户信息
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.user.Entity GetUserEntity(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return GetUserEntity(login.Web, uid);
        }

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.user.Entity GetUserEntity(string uid)
        {
            return GetWeiboUserEntity(uid);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public Collection GetUserEntityColloction(string appKey, string uids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appKey"></param>
        /// <param name="uids"></param>
        /// <returns></returns>
        public Dictionary<string, IEnumerable<Tag>> GetTags(string appKey, string uids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据用户uid获取最近10条微博
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return GetMblogs(login.Web, uid);
        }

        /// <summary>
        /// 根据UID获取第一页博文列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(string uid)
        {
            return BowenSpiderExecute(uid);
        }

        /// <summary>
        /// 获取指定页码的微博
        /// 遇到指定的mid则不再查找
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">被查找的uid</param>
        /// <param name="mid">小于该指定的mid不再查找</param>
        /// <param name="page">查找页数</param>
        /// <returns></returns>
        List<CommonEntityLib.Entities.status.Entity> IHttpWork.GetMblogs(WebAccessBase web, string uid, string mid, int page)
        {
            return GetMblogs(web, uid, mid, page);
        }

        /// <summary>
        /// 根据uid判断用户是否存在
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool UserExist(IWeiboLogin webLogin, string uid)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return UserExist(login.Web, uid);
        }

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool UserExist(string uid)
        {
            return Exist(uid);
        }

        /// <summary>
        /// 获取新增粉丝的第一页
        /// </summary>
        /// <param name="webLogin"></param>
        public List<string> GetNewFans(IWeiboLogin webLogin)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return GetNewFans(login.Web);
        }

        /// <summary>
        /// 根据m.weibo.cn博文地址获取博文信息
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.status.Entity GetMblog(IWeiboLogin webLogin, string url)
        {
            CNWeiboLogin login = webLogin.GetWeiboLogin<CNWeiboLogin>();
            return GetMblog(login.Web, url);
        }

        /// <summary>
        /// 获取最后的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>最后的关注UID列表</returns>
        public List<long> GetLastAttentionUIDs(string uid)
        {
            return AttentionSpiderExecute1(uid);
        }

        /// <summary>
        /// 获取最前的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>获取最前的关注UID列表</returns>
        public List<long> GetPrewAttentionUIDs(string uid)
        {
            return AttentionSpiderExecute(uid);
        }

        /// <summary>
        /// 获取20个fans
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>20个fans</returns>
        List<long> IHttpWork.Get20Fans(string uid)
        {
            return Get20Fans(uid);
        }

        /// <summary>
        /// 并行获取关注的UID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<long> GetAttentionUIDsAsParallel(string uid)
        {
            return AttentionSpiderParallelExecute(uid);
        }

        /// <summary>
        /// 用户信息搜索
        /// </summary>
        /// <param name="factor1">类型：只搜公司 1 只搜学校 2 只搜标签 3</param>
        /// <param name="factor2">性别：男性 4  女性 5</param>
        /// <param name="factor3">用户类型：机构认证用户 6 个人认证用户 7 普通用户 8</param>
        /// <param name="factor4">年龄段：18以下_9 19到22_10 23到29_11 30到39_12 40以上_13</param>
        /// <param name="queryWord">查询字符串</param>
        /// <returns>符合条件的UserID列表</returns>
        List<string> IHttpWork.GetWeiboHttpSearchUserIDs(int factor1, int factor2, int factor3, int factor4, string queryWord)
        {
            return GetWeiboHttpSearchUserIDs(factor1, factor2, factor3, factor4, queryWord);
        }

        #region SinaWeibo.HttpWorkByMcn

        private static readonly Regex UidRegex = new Regex(@"/(?<uid>\d{1,10})/");

        private static readonly Regex NicknameRegex = new Regex("<span>昵称</span><p>(?<nickname>.*?)</p>");
        private static readonly Regex GenderRegex = new Regex("<span>性别</span><p>(?<gender>.*?)</p>");
        private static readonly Regex LocationRegex = new Regex("<span>所在地</span><p>(?<location>.*?)</p>");
        private static readonly Regex DescriptionRegex = new Regex("<span>简介</span><p>(?<description>.*?)</p>");

        private static readonly Regex HtmlRegex = new Regex("<[^>]+>");

        private static readonly string ProvinceAndCity = File.ReadAllText("provinceandcity.txt");

        #region 发微博 AddMblog

        /// <summary>
        /// 发微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="text"></param>
        /// <param name="pic">picID</param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public static string AddMblog(WebAccessBase web, string text, string pic = null, string appkey = null)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text", "微博内容不能为空");

            if (Encoding.GetEncoding("GBK").GetByteCount(text) > 280)
                return "文本内容超出140字";

            web.Encode = Encoding.UTF8;
            web.Reffer = new Uri("http://m.weibo.cn/");
            web.GetHTML("http://m.weibo.cn/mblog");
            const string postUrl = "http://m.weibo.cn/mblogDeal/addAMblog";
            var postData = string.IsNullOrEmpty(pic)
                ? string.Format("content={0}", Uri.EscapeDataString(text))
                : string.Format("content={0}&picId={1}", Uri.EscapeDataString(text), pic);
            if (!string.IsNullOrEmpty(appkey))
            {
                postData += string.Format("&appkey={0}", appkey);
            }
            var html = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //发微博结果分析
            string res = html.CommonAnalyse("发微博");
            return res;
        }

        #endregion

        #region 转发评论 RetweetMblog | CommentMblog

        /// <summary>
        /// 转发
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">被转发的URL(例如：http://m.weibo.cn/5538854138/CeLep9umj )</param>
        /// <param name="id">被转发的微博ID</param>
        /// <param name="text">转发内容</param>
        /// <param name="isComment">是否同时评论(默认否)</param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public static string RetweetMblog(WebAccessBase web, string url, string id, string text, bool isComment = false, string appkey = null)
        {
            CheckRetweetAndCommentMblogArgument(web, url, id, text);
            web.Encode = Encoding.UTF8;
            //访问微博页
            web.Reffer = null;
            var weiboHtml = web.GetHTML(url);
            if (string.IsNullOrEmpty(weiboHtml))
            {
                CNHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 转发";
            }
            if (weiboHtml.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
            {
                return "被转发的微博不存在";
            }
            //访问转发提交页
            var retweetWorkUrl = string.Format("http://m.weibo.cn/repost?id={0}", id);
            var retweetWorkHtml = web.GetHTML(retweetWorkUrl);
            if (string.IsNullOrEmpty(retweetWorkHtml))
            {
                CNHttpWorkLogger.Info("访问转发提交页面{0}出错", id);
                return "网络错误 转发";
            }
            //转发
            const string postUrl = "http://m.weibo.cn/mblogDeal/rtMblog";
            var postData = string.Format("content={0}&id={1}", Uri.EscapeDataString(text), id);
            //转发带评论
            if (isComment)
            {
                var uid = UidRegex.Match(url).Groups["uid"].Value;
                postData += string.Format("&rtcomment={0}", uid);
            }
            if (!string.IsNullOrEmpty(appkey))
            {
                postData += string.Format("&source={0}", appkey);
            }
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //转发微博结果分析
            string res = postHtml.CommonAnalyse("转发微博");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        /// <summary>
        /// 评论
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">被评论的URL(例如：http://m.weibo.cn/5538854138/CeLep9umj )</param>
        /// <param name="id">被评论的微博ID</param>
        /// <param name="text">评论内容</param>
        /// <param name="isRetweet">评论同时带转发</param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public static string CommentMblog(WebAccessBase web, string url, string id, string text, bool isRetweet = false, string appkey = null)
        {
            CheckRetweetAndCommentMblogArgument(web, url, id, text);
            web.Encode = Encoding.UTF8;
            //访问微博页
            web.Reffer = null;
            var weiboHtml = web.GetHTML(url);
            if (string.IsNullOrEmpty(weiboHtml))
            {
                CNHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 评论";
            }
            if (weiboHtml.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
            {
                return "被评论的微博不存在";
            }
            //访问评论提交页
            var retweetWorkUrl = string.Format("http://m.weibo.cn/comment?id={0}", id);
            var retweetWorkHtml = web.GetHTML(retweetWorkUrl);
            if (string.IsNullOrEmpty(retweetWorkHtml))
            {
                CNHttpWorkLogger.Info("访问评论提交页面{0}出错", id);
                return "网络错误 评论";
            }
            //评论
            const string postUrl = "http://m.weibo.cn/commentDeal/addCmt";
            var postData = string.Format("content={0}&id={1}", Uri.EscapeDataString(text), id);
            if (!string.IsNullOrEmpty(appkey))
            {
                postData += string.Format("&appkey={0}", appkey);
            }
            if (isRetweet)
            {
                postData += "&rt=1";
            }
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //转发微博结果分析
            string res = postHtml.CommonAnalyse("评论微博");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        /// <summary>
        /// 检测转发参数
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url"></param>
        /// <param name="mid"></param>
        /// <param name="text"></param>
        private static void CheckRetweetAndCommentMblogArgument(WebAccessBase web, string url, string mid, string text)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url");
            if (string.IsNullOrEmpty(mid))
                throw new ArgumentNullException("mid");
            if (string.IsNullOrEmpty(text))
                throw new ArgumentNullException("text");
        }

        #endregion

        #region 关注 FriendCreate | FriendDestroy

        /// <summary>
        /// 加关注
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string FriendCreate(WebAccessBase web, string uid)
        {
            CheckFriendArgument(web, uid);
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", uid));
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("访问关注对象{0}页面出错", uid);
                return "网络错误 关注";
            }
            if (home.Contains("<title>相关信息</title>") && home.Contains(@"\u8fd9\u91cc\u8fd8\u6ca1\u6709\u5185\u5bb9"))
            {
                return "工作对象被封";
            }
            var postUrl = string.Format("http://m.weibo.cn/attentionDeal/addAttention?");
            var postData = string.Format("uid={0}", uid);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //关注结果分析
            string res = postHtml.CommonAnalyse("关注");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string FriendDestroy(WebAccessBase web, string uid)
        {
            CheckFriendArgument(web, uid);
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", uid));
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("访问取消关注对象{0}页面出错", uid);
                return "网络错误 取消关注";
            }
            if (home.Contains("<title>相关信息</title>") && home.Contains(@"\u8fd9\u91cc\u8fd8\u6ca1\u6709\u5185\u5bb9"))
            {
                return "工作对象被封";
            }
            var postUrl = string.Format("http://m.weibo.cn/attentionDeal/delAttention");
            var postData = string.Format("&uid={0}", uid);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //取消关注结果分析
            string res = postHtml.CommonAnalyse("取消关注");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        private static void CheckFriendArgument(WebAccessBase web, string uid)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid", "Uid不能为空");
        }

        #endregion

        #region 点赞 AttitudesCreate | AttitudesDestroy

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">微博的URL(例如：http://m.weibo.cn/5538854138/CeLep9umj )</param>
        /// <param name="id">微博的id "CeLep9umj".MidToId()</param>
        /// <returns></returns>
        public static string AttitudesCreate(WebAccessBase web, string url, string id)
        {
            CheckAttitudesArgument(web, url, id);
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(url);
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 点赞";
            }
            if (home.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
            {
                return "工作对象被封";
            }
            var postUrl = string.Format("http://m.weibo.cn/attitudesDeal/add");
            var postData = string.Format("id={0}&attitude=heart", id);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //点赞结果分析
            string res = postHtml.CommonAnalyse("点赞");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        /// <summary>
        /// 取消点赞
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">微博的URL(例如：http://m.weibo.cn/5538854138/CeLep9umj )</param>
        /// <param name="id">微博的id "CeLep9umj".MidToId()</param>
        /// <returns></returns>
        public static string AttitudesDestroy(WebAccessBase web, string url, string id)
        {
            CheckAttitudesArgument(web, url, id);
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(url);
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 取消点赞";
            }
            if (home.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
            {
                return "工作对象被封";
            }
            var postUrl = string.Format("http://m.weibo.cn/attitudesDeal/delete");
            var postData = string.Format("id={0}", id);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //点赞结果分析
            string res = postHtml.CommonAnalyse("取消点赞");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        private static void CheckAttitudesArgument(WebAccessBase web, string weiboUrl, string id)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(weiboUrl))
                throw new ArgumentNullException("weiboUrl", "微博url不能为空");
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException("id", "微博id不能为空");
        }

        #endregion

        #region 复制被克隆对象信息 SaveInformation*

        /// <summary>
        /// 复制被克隆对象信息
        /// </summary>
        /// <param name="web"></param>
        /// <param name="targetUid"></param>
        /// <param name="uid">小号的uid</param>
        /// <param name="nickname">小号待更新的昵称,若为空则不不修改</param>
        /// <returns></returns>
        public static string SaveInformation(WebAccessBase web, string targetUid, string uid, string nickname)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(targetUid))
                throw new ArgumentNullException("targetUid", "目标用户UID不能为空");
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", targetUid));
            var targetInfoHtml = web.GetHTML(string.Format("http://m.weibo.cn/users/{0}?", targetUid));
            if (string.IsNullOrEmpty(targetInfoHtml))
            {
                CNHttpWorkLogger.Info("访问{0}信息页面出错", targetUid);
                return "网络错误 复制信息";
            }
            CommonEntityLib.Entities.user.Entity entity = AnalyseUserPage(targetInfoHtml);
            web.Reffer = new Uri("http://m.weibo.cn/");
            web.GetHTML("http://m.weibo.cn/home/setting");
            web.GetHTML(string.Format("http://m.weibo.cn/users/{0}?set=1", uid));
            const string postUrl = "http://m.weibo.cn/settingDeal/inforSave";
            const string birthdayStr = "&year=00&month=00&day=00";

            //描述只能140字节，这里截取到140
            string description = entity.Description;
            if (Encoding.GetEncoding("GBK").GetByteCount(description) > 140)
            {
                do
                {
                    description = description.Substring(0, description.Length - 1);
                    if (Encoding.GetEncoding("GBK").GetByteCount(description) <= 140)
                        break;
                } while (true);
            }

            string postData =
             string.Format(
                 "gender={1}&province={2}&city={3}{4}{0}&email=&url=&qq=&msn=&description={5}",
                 string.IsNullOrEmpty(nickname) ? "" : "&screen_name=" + Uri.EscapeDataString(nickname),
                 entity.Gender, entity.Province, entity.City, birthdayStr, Uri.EscapeDataString(description));

            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //复制信息结果分析
            string res = postHtml.CommonAnalyse("复制信息");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        private static CommonEntityLib.Entities.user.Entity AnalyseUserPage(string html)
        {
            var location = LocationRegex.Match(html).Groups["location"].Value;
            string province;
            string city;
            if (!string.IsNullOrEmpty(location))
            {
                province = location.Split(' ')[0];
                province = ProvinceToCode(province);
                city = "0";
                if (location.Contains(" "))
                {
                    city = location.Split(' ')[1];
                    city = CityToCode(city);
                }
            }
            else
            {
                province = "11";
                city = "0";
            }
            CommonEntityLib.Entities.user.Entity userInformationEntity = new CommonEntityLib.Entities.user.Entity
            {
                ScreenName = NicknameRegex.Match(html).Groups["nickname"].Value,
                Gender = GenderRegex.Match(html).Groups["gender"].Value == "男" ? "m" : "f",
                Description = DescriptionRegex.Match(html).Groups["description"].Value,
                Province = province,
                City = city
            };
            return userInformationEntity;
        }

        private static string ProvinceToCode(string province)
        {
            string pattern = string.Format("<province id=\"(?<code>.*?)\" name=\"{0}\">", province);
            return Regex.Match(ProvinceAndCity, pattern).Groups["code"].Value;
        }

        private static string CityToCode(string city)
        {
            string pattern = string.Format("<city id=\"(?<code>.*?)\" name=\"{0}\"/>", city);
            return Regex.Match(ProvinceAndCity, pattern).Groups["code"].Value;
        }

        #endregion

        #region 发私信 SendDirectMessages | RecevieDirectMessages*

        /// <summary>
        /// 发私信
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">私信对象</param>
        /// <param name="text">私信内容</param>
        /// <returns></returns>
        public static string SendDirectMessages(WebAccessBase web, string uid, string text)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", uid));
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("访问私信对象{0}页面出错", uid);
                return "网络错误 发私信";
            }
            if (home.Contains("<title>相关信息</title>") && home.Contains(@"\u8fd9\u91cc\u8fd8\u6ca1\u6709\u5185\u5bb9"))
            {
                return string.Format("私信对象{0}对封", uid);
            }
            //访问私信提交页
            var sixinWorkUrl = string.Format("http://m.weibo.cn/msg/chat?uid={0}", uid);
            var sixinWorkHtml = web.GetHTML(sixinWorkUrl);
            if (string.IsNullOrEmpty(sixinWorkHtml))
            {
                CNHttpWorkLogger.Info("访问{0}的私信提交页面出错", uid);
                return "网络错误 发私信";
            }
            var postUrl = string.Format("http://m.weibo.cn/msgDeal/sendMsg?");
            var postData = string.Format("fileId=null&uid={0}&content={1}", uid, Uri.EscapeDataString(text));
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //私信结果分析
            string res = postHtml.CommonAnalyse("私信");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        /// <summary>
        /// 收发给自己的私信
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">账号自己的Uid</param>
        public static List<CnDirectMessagesEntity> RecevieDirectMessages(WebAccessBase web, string uid)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            List<CnDirectMessagesEntity> result = new List<CnDirectMessagesEntity>();
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML("http://m.weibo.cn/");
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("私信{0}访问自己主页为空", uid);
                return null;
            }
            var listStr = web.GetHTML("http://m.weibo.cn/msg/index?format=cards");
            try
            {
                dynamic listDynamic = DynamicJson.Parse(listStr);
                var list = listDynamic[0].card_group;

                foreach (var item in list)
                {
                    if (!item.IsDefined("text")) continue;
                    if (string.IsNullOrEmpty(item.text)) continue;
                    if (!item.IsDefined("unread") || item.unread == "0") continue;

                    var sendUid = item.user.id;

                    CnDirectMessagesEntity cnDirectMessagesEntity = new CnDirectMessagesEntity
                    {
                        Time = CommonExtension.AnalyseTime(item.created_at),
                        Text = item.text,
                        Uid = sendUid,
                        Nickname = item.user.screen_name
                    };
                    if (CheckSendUser(web, sendUid))
                    {
                        result.Add(cnDirectMessagesEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                CNHttpWorkLogger.Error(string.Format("{0}分析私信列表失败\r\n{1}", uid, listStr), ex);
            }

            web.Reffer = new Uri("http://m.weibo.cn/msg/notes");
            var listStr2 = web.GetHTML("http://m.weibo.cn/msg/noteList?page=1");
            try
            {
                dynamic listDynamic = DynamicJson.Parse(listStr2);
                if (listDynamic.ok == "1")
                {
                    var list = listDynamic.data;

                    foreach (var item in list)
                    {
                        CnDirectMessagesEntity cnDirectMessagesEntity = new CnDirectMessagesEntity
                        {
                            Time = CommonExtension.AnalyseTime(item.Value.time),
                            Text = item.Value.text,
                            Uid = item.Value.sender.uid,
                            Nickname = item.Value.sender.name
                        };
                        result.Add(cnDirectMessagesEntity);
                    }
                }
            }
            catch (Exception ex)
            {
                CNHttpWorkLogger.Error(string.Format("{0}分析未关注联系人私信列表失败\r\n{1}", uid, listStr), ex);
            }

            return result.OrderByDescending(p => p.Time).ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">要检测的Uid</param>
        /// <returns></returns>
        private static bool CheckSendUser(WebAccessBase web, string uid)
        {
            web.Reffer = new Uri(string.Format("http://m.weibo.cn/msg/chat?uid={0}", uid));
            var url = string.Format("http://m.weibo.cn/msg/messages?uid={0}&page=1", uid);
            var html = web.GetHTML(url);
            if (string.IsNullOrEmpty(html))
                return false;

            try
            {
                dynamic htmlDynamic = DynamicJson.Parse(html);
                foreach (var item in htmlDynamic.data)
                {
                    return item.sender.id == uid;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 收所有的私信会话
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<CnDirectMessagesEntity> RecevieAllDirectMessages(WebAccessBase web, string uid)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            List<CnDirectMessagesEntity> result = new List<CnDirectMessagesEntity>();
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML("http://m.weibo.cn/");
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("私信{0}访问自己主页为空", uid);
                return null;
            }
            var listStr = web.GetHTML("http://m.weibo.cn/msg/index?format=cards");
            try
            {
                dynamic listDynamic = DynamicJson.Parse(listStr);
                var list = listDynamic[0].card_group;
                foreach (var item in list)
                {
                    if (!item.IsDefined("text")) continue;
                    var dialogUid = item.user.id;
                    var dialogs = GetDialogs(web, dialogUid);
                    result.AddRange(dialogs);
                }
            }
            catch (Exception)
            {
                CNHttpWorkLogger.Info("{0}分析私信列表失败\r\n{1}", uid, listStr);
            }

            web.Reffer = new Uri("http://m.weibo.cn/msg/notes");
            var listStr2 = web.GetHTML("http://m.weibo.cn/msg/noteList?page=1");
            try
            {
                dynamic listDynamic = DynamicJson.Parse(listStr2);
                if (listDynamic.ok == "1")
                {
                    var list = listDynamic.data;

                    foreach (var item in list)
                    {
                        string text = item.Value.text;
                        text = HtmlRegex.Replace(text, "");
                        CnDirectMessagesEntity cnDirectMessagesEntity = new CnDirectMessagesEntity
                        {
                            Time = CommonExtension.AnalyseTime(item.Value.time),
                            Text = text,
                            Uid = item.Value.sender.uid,
                            Nickname = item.Value.sender.name
                        };
                        result.Add(cnDirectMessagesEntity);
                    }
                }
            }
            catch (Exception)
            {
                CNHttpWorkLogger.Info("{0}分析未关注联系人私信列表失败\r\n{1}", uid, listStr);
            }
            return result.OrderByDescending(p => p.Time).ToList();
        }

        /// <summary>
        /// 获取指定Uid与当前登录账号的私信会话
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        private static IEnumerable<CnDirectMessagesEntity> GetDialogs(WebAccessBase web, string uid)
        {
            var referer = string.Format("http://m.weibo.cn/msg/chat?uid={0}", uid);
            web.Reffer = new Uri(referer);
            var url = string.Format("http://m.weibo.cn/msg/messages?uid={0}&page=1", uid);
            var html = web.GetHTML(url);
            if (string.IsNullOrEmpty(html))
                return new List<CnDirectMessagesEntity>();
            try
            {
                var result = new List<CnDirectMessagesEntity>();
                dynamic json = DynamicJson.Parse(html);
                if (json.IsDefined("data"))
                {
                    foreach (var item in json.data)
                    {
                        string text = item.text;
                        text = HtmlRegex.Replace(text, "");
                        CnDirectMessagesEntity cnDirectMessagesEntity = new CnDirectMessagesEntity
                        {
                            Time = CommonExtension.AnalyseTime(item.created_at),
                            Text = text,
                            Uid = item.sender.id,
                            Nickname = item.sender.screen_name
                        };
                        result.Add(cnDirectMessagesEntity);
                    }
                    return result;
                }
            }
            catch (Exception)
            {
                CNHttpWorkLogger.Info("分析{0}的私信会话列表失败\r\n{1}", uid, html);
            }
            return new List<CnDirectMessagesEntity>();
        }


        #endregion

        #region 评论 SendReplyComment | ReceiveComment*

        /// <summary>
        /// 发送回复评论
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">工作号自己的Uid（用于记录日志）</param>
        /// <param name="mid">评论的那条微博mid</param>
        /// <param name="cid">被回复的评论cid</param>
        /// <param name="replyName">被回复人的昵称</param>
        /// <param name="replyContent">回复评论内容</param>
        /// <returns></returns>
        public static string SendReplyComment(WebAccessBase web, string uid, string mid, string cid, string replyName, string replyContent)
        {
            CheckSendReplyCommentArgument(web, mid, cid, replyName, replyContent);
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML("http://m.weibo.cn/");
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("回复评论{0}访问自己主页为空", uid);
                return "网络错误 回复评论";
            }
            var tempHtml = web.GetHTML("http://m.weibo.cn/msg/cmts?subtype=allPL");
            if (string.IsNullOrEmpty(tempHtml))
            {
                CNHttpWorkLogger.Info("回复评论{0}访问自己评论列表页面为空", uid);
                return "网络错误 回复评论";
            }
            string url = string.Format("http://m.weibo.cn/comment?id={0}&reply={1}&content=回复@{2}:", mid, cid, replyName);
            tempHtml = web.GetHTML(url);
            if (string.IsNullOrEmpty(tempHtml))
            {
                CNHttpWorkLogger.Info("回复评论{0}访问回复评论页面{1}为空", uid, mid);
                return "网络错误 回复评论";
            }
            const string postUrl = "http://m.weibo.cn/commentDeal/replyComment";
            string postData = string.Format("content=回复@{2}:{3}&id={0}&cid={1}", mid, cid, replyName, Uri.EscapeDataString(replyContent));
            string postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            string res = postHtml.CommonAnalyse("回复评论");
            if (!string.IsNullOrEmpty(res))
            {
                CNHttpWorkLogger.Info(res);
            }
            return res;
        }

        private static void CheckSendReplyCommentArgument(WebAccessBase web, string mid, string cid, string replyName, string replyContent)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(mid))
                throw new ArgumentNullException("mid", "微博MID不能为空");
            if (string.IsNullOrEmpty(cid))
                throw new ArgumentNullException("cid", "评论CID不能为空");
            if (string.IsNullOrEmpty(replyName))
                throw new ArgumentNullException("replyName", "回复评论对象的昵称不能为空");
            if (string.IsNullOrEmpty(replyContent))
                throw new ArgumentNullException("replyContent", "回复评论内容不能为空");
        }

        /// <summary>
        /// 收发给自己的评论
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<Entity> ReceiveComment(WebAccessBase web, string uid)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            List<Entity> result = new List<Entity>();
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML("http://m.weibo.cn/");
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("用户{0}获取自己主页为空", uid);
                return null;
            }
            var allPlStr = web.GetHTML("http://m.weibo.cn/msg/cmts?subtype=allPL");
            if (string.IsNullOrEmpty(home))
            {
                CNHttpWorkLogger.Info("用户{0}获取自己评论页为空", uid);
                return null;
            }
            try
            {
                string jsonStr = GetHomeCommentJsonStr(allPlStr);
                dynamic json = DynamicJson.Parse(jsonStr);
                foreach (var item in json.card_group)
                {
                    if (string.IsNullOrEmpty(item.text)) continue;

                    var text = item.text;
                    text = HtmlRegex.Replace(text, "");
                    if (string.IsNullOrEmpty(text)) continue;

                    Entity cnCommentEntity = new Entity
                    {
                        MID = item.card.page_id,
                        //Url = item.card.page_url,
                        CreatedAt = CommonExtension.AnalyseTime(item.created_at).ToString(),
                        ID = item.id, //Cid -->ID
                        Text = text,
                        User = new CommonEntityLib.Entities.user.Entity
                        {
                            ID = item.user.id,
                            ScreenName = item.user.screen_name
                        }
                        //Uid = item.user.id,
                        //Name = item.user.screen_name,
                        //Time = item.created_at.AnalyseTime()
                    };
                    result.Add(cnCommentEntity);
                }
            }
            catch (Exception ex)
            {
                CNHttpWorkLogger.Error(string.Format("{0}分析评论列表失败\r\n{1}", uid, allPlStr), ex);
            }
            return result.OrderByDescending(p => DateTime.Parse(p.CreatedAt)).ToList();
        }

        /// <summary>
        /// 获取主页里面
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string GetHomeCommentJsonStr(string html)
        {
            var index = html.IndexOf(@"{""mod_type"":""mod\/pagelist""", StringComparison.Ordinal);
            html = html.Substring(index);

            int last = 0;
            Stack<char> stack = new Stack<char>();
            for (int i = 0; i < html.Length; i++)
            {
                if (html[i] == '{')
                {
                    stack.Push(html[i]);
                }
                if (html[i] == '}')
                {
                    stack.Pop();
                }
                if (stack.Count == 0)
                {
                    last = i;
                    break;
                }
            }
            return html.Substring(0, last + 1);
        }

        #endregion

        #region 用户 GetUserEntity* | GetMblogs* | UserExist | GetMblog* | GetNewFans

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static CommonEntityLib.Entities.user.Entity GetUserEntity(WebAccessBase web, string uid)
        {
            try
            {
                var homeStr = GetCnPage.GetUser(web, uid);
                var result = AnalyseCnPage.AnalysisUserHome(homeStr);
                return result;
            }
            catch (Exception exception)
            {
                CNHttpWorkLogger.Error(string.Format("获取用户{0}主页发生异常", uid), exception);
                return null;
            }
        }

        /// <summary>
        /// 根据用户uid获取最近10条微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<CommonEntityLib.Entities.status.Entity> GetMblogs(WebAccessBase web, string uid)
        {
            try
            {
                var userHome = GetCnPage.GetUser(web, uid);
                var weiboUser = AnalyseCnPage.AnalysisUserHome(userHome);
                if (weiboUser != null && weiboUser.StatusesCount > 0)
                {
                    //对于网络错误进行重试
                    var pageStr = GetCnPage.GetMultiMblog(web, weiboUser, 1);
                    var list = AnalyseCnPage.AnalysisMultiWeiboPage(pageStr);
                    //移除空值
                    list.RemoveAll(p => p == null);
                    //移除置顶微博
                    if (list.Count >= 2)
                    {
                        if (long.Parse(list[0].ID) < long.Parse(list[1].ID))
                        {
                            list.RemoveAt(0);
                        }
                    }
                    //移除空值
                    list.RemoveAll(p => p == null);
                    foreach (var mblog in list)
                    {
                        if (mblog != null && !string.IsNullOrEmpty(mblog.Text))
                        {
                            //去掉HTML标签
                            mblog.Text = HtmlRegex.Replace(mblog.Text, "");
                        }
                    }
                    return list;
                }
            }
            catch (Exception exception)
            {
                CNHttpWorkLogger.Error(string.Format("UID:{0}获取博文页面发生异常", uid), exception);
            }

            return new List<CommonEntityLib.Entities.status.Entity>();
        }

        /// <summary>
        /// 获取指定页码的微博
        /// 遇到指定的mid则不再查找
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">被查找的uid</param>
        /// <param name="mid">小于该指定的mid不再查找</param>
        /// <param name="page">查找页数</param>
        /// <returns></returns>
        public static List<CommonEntityLib.Entities.status.Entity> GetMblogs(WebAccessBase web, string uid, string mid, int page)
        {
            try
            {
                var result = new List<CommonEntityLib.Entities.status.Entity>();

                var userHome = GetCnPage.GetUser(web, uid);
                var weiboUser = AnalyseCnPage.AnalysisUserHome(userHome);
                if (weiboUser != null && weiboUser.StatusesCount > 0)
                {
                    for (int i = 1; i <= page; i++)
                    {
                        var pageStr = GetCnPage.GetMultiMblog(web, weiboUser, i);
                        if (string.IsNullOrEmpty(pageStr))
                            return new List<CommonEntityLib.Entities.status.Entity>();
                        var list = AnalyseCnPage.AnalysisMultiWeiboPage(pageStr);
                        //移除空值
                        list.RemoveAll(p => p == null);
                        //移除置顶微博
                        if (list.Count >= 2)
                        {
                            if (long.Parse(list[0].ID) < long.Parse(list[1].ID))
                            {
                                list.RemoveAt(0);
                            }
                        }
                        //移除空值
                        list.RemoveAll(p => p == null);
                        foreach (var mblog in list)
                        {
                            long mblogID;
                            if (long.TryParse(mblog.ID, out mblogID) && mblogID <= long.Parse(mid))
                                return result;
                            if (!string.IsNullOrEmpty(mblog.Text))
                            {
                                //去掉HTML标签
                                mblog.Text = HtmlRegex.Replace(mblog.Text, "");
                                result.Add(mblog);
                            }
                        }
                    }
                    return result;
                }
            }
            catch (Exception exception)
            {
                CNHttpWorkLogger.Error(string.Format("UID:{0}获取博文页面发生异常", uid), exception);
            }

            return new List<CommonEntityLib.Entities.status.Entity>();
        }

        /// <summary>
        /// 根据uid判断用户是否存在
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool UserExist(WebAccessBase web, string uid)
        {
            for (int i = 0; i < 10; i++)
            {
                web.Reffer = null;
                var home = web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", uid));
                if (string.IsNullOrEmpty(home))
                {
                    continue;
                }
                if (home.Contains("<title>相关信息</title>") && home.Contains(@"\u8fd9\u91cc\u8fd8\u6ca1\u6709\u5185\u5bb9"))
                {
                    return false;
                }
                if (home.Contains(uid))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 根据m.weibo.cn博文地址获取博文信息
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static CommonEntityLib.Entities.status.Entity GetMblog(WebAccessBase web, string url)
        {
            try
            {
                var mblogStr = GetCnPage.GetSingleMblog(web, url);
                if (!string.IsNullOrEmpty(mblogStr) && mblogStr.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
                {
                    return null;
                }
                var result = AnalyseCnPage.AnalysisSingleWeiboPage(mblogStr);
                return result;
            }
            catch (Exception exception)
            {
                CNHttpWorkLogger.Error(string.Format("获取{0}博文页面发生异常", url), exception);
                return null;
            }
        }

        /// <summary>
        /// 获取新增粉丝的第一页
        /// </summary>
        /// <param name="web"></param>
        public static List<string> GetNewFans(WebAccessBase web)
        {
            var newFansPage = GetCnPage.GetNewFans(web);
            if (string.IsNullOrEmpty(newFansPage))
            {
                CNHttpWorkLogger.Info("访问新增粉丝页面出错");
                return new List<string>();
            }
            return AnalyseCnPage.AnalysisNewFansPage(newFansPage);
        }

        #endregion

        /// <summary>
        /// 删除微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">微博地址(m.weibo.cn)</param>
        /// <param name="mid">微博ID</param>
        /// <returns></returns>
        public static string DelMblog(WebAccessBase web, string url, string mid)
        {
            web.Reffer = new Uri("http://m.weibo.cn/");
            var weiboStr = web.GetHTML(url);
            if (string.IsNullOrEmpty(weiboStr))
            {
                CNHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 删除微博";
            }
            if (weiboStr.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
            {
                return "微博不存在";
            }
            var result = web.PostWithHeaders("http://m.weibo.cn/mblogDeal/delMyMblog", "id=" + mid,
                  new[] { "X-Requested-With: XMLHttpRequest" });
            return result.CommonAnalyse("删除微博");
        }

        /// <summary>
        /// 上传头像
        /// </summary>
        /// <param name="web"></param>
        /// <param name="cloneUid"></param>
        /// <returns></returns>
        public static string UploadAvatar(WebAccessBase web, string cloneUid)
        {
            var web2 = new WebAccessBase();
            var userStr = GetCnPage.GetUser(web2, cloneUid);
            var user = AnalyseCnPage.AnalysisUserHome(userStr);
            if (user == null)
            {
                return "上传头像 访问克隆对象页面失败";
            }
            var avatarurl = user.ProfileImageUrl;
            byte[] picBytes = web2.GetImageByte(avatarurl);
            if (picBytes == null)
            {
                return "上传头像 访问克隆对象头像失败";
            }
            File.WriteAllBytes("0.jpg", picBytes);
            web.Reffer = null;
            web.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36";
            var html1 = web.GetHTML("http://weibo.cn");
            if (string.IsNullOrEmpty(html1))
            {
                return "上传头像 访问主页失败";
            }
            var setUrl = Regex.Match(html1, @"<a  href=""(?<url>.*?)"">设置</a>").Groups["url"].Value;
            if (string.IsNullOrEmpty(setUrl))
            {
                return "上传头像 分析设置页地址失败";
            }
            setUrl = "http://weibo.cn" + setUrl.Replace("&amp;", "&");
            var html2 = web.GetHTML(setUrl);
            if (string.IsNullOrEmpty(html2))
            {
                return "上传头像 访问设置页失败";
            }
            var dataUrl = Regex.Match(html2, @"<a href=""(?<url>[^""]*?)"">资料</a>").Groups["url"].Value;
            if (string.IsNullOrEmpty(dataUrl))
            {
                return "上传头像 分析资料页地址失败";
            }
            dataUrl = "http://weibo.cn" + dataUrl;
            var html3 = web.GetHTML(dataUrl);
            if (string.IsNullOrEmpty(html3))
            {
                return "上传头像 访问资料页失败";
            }
            var avatarUrl = Regex.Match(html3, @"<a href=""(?<url>[^""]*?)"">头像</a>").Groups["url"].Value;
            if (string.IsNullOrEmpty(avatarUrl))
            {
                return "上传头像 分析头像页地址失败";
            }
            avatarUrl = "http://weibo.cn" + avatarUrl;
            var html4 = web.GetHTML(avatarUrl);
            if (string.IsNullOrEmpty(html4))
            {
                return "上传头像 访问头像页失败";
            }
            var avatarPostUrl = Regex.Match(html4, @"form action=""(?<url>[^""]*?)""").Groups["url"].Value;
            if (string.IsNullOrEmpty(avatarPostUrl))
            {
                return "上传头像 分析头像上传地址失败";
            }
            avatarPostUrl = "http://weibo.cn" + avatarPostUrl.Replace("&amp;", "&");
            var result = web.UploadImage(picBytes, avatarPostUrl, "pic", "image/jpeg",
                 "Content-Disposition: form-data; name=\"act\"\r\n\r\navatar&Content-Disposition: form-data; name=\"save\"\r\n\r\n1");
            if (!string.IsNullOrEmpty(result) && result.Contains("<div class=\"ps\">设置成功</div>"))
                return "";
            CNHttpWorkLogger.Info("上传头像失败\r\n{0}", result);
            return "上传头像 上传失败";
        }

        #endregion

        #region SinaWeibo.CnUnfreeze

        private static readonly Regex StRegex = new Regex("<script>var st = '(?<st>.*?)';</script>");

        private static readonly Regex CnUnfreezeUidRegex = new Regex(@"""id"":""(?<uid>.*?)""");

        private const string UnfreezeByAvatarQuickNextPostUrl = "http://m.weibo.cn/security/quickNext";
        private const string UnfreezeByAvatarQuickUnfreezePostUrl = "http://m.weibo.cn/security/quickUnfreeze";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string UnfreezeByAvatar(WebAccessBase web)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            var home = web.GetHTML("http://m.weibo.cn/security?");
            if (string.IsNullOrEmpty(home))
            {
                return "访问安全页面失败";
            }
            var st = StRegex.Match(home).Groups["st"].Value;
            if (string.IsNullOrEmpty(st))
            {
                return "分析st失败";
            }
            web.Reffer = new Uri("http://m.weibo.cn/security?");
            string first = web.Post(UnfreezeByAvatarQuickNextPostUrl, string.Format("number=0&st={0}", st));
            if (string.IsNullOrEmpty(first))
            {
                return "第一次提交失败";
            }
            int firstCode = AnalyseNextNumber(first);
            if (firstCode == -1)
            {
                return "分析第一次用户失败";
            }
            web.Reffer = new Uri("http://m.weibo.cn/security?");
            string second = web.Post(UnfreezeByAvatarQuickNextPostUrl, string.Format("number={1}&st={0}", st, firstCode));
            if (string.IsNullOrEmpty(second))
            {
                return "第二次提交失败";
            }
            int secondCode = AnalyseNextNumber(second);
            if (secondCode == -1)
            {
                return "分析第二次用户失败";
            }
            web.Reffer = new Uri("http://m.weibo.cn/security?");
            string third = web.Post(UnfreezeByAvatarQuickNextPostUrl, string.Format("number={1}&st={0}", st, secondCode));
            if (string.IsNullOrEmpty(third))
            {
                return "第二次提交失败";
            }
            int thirdCode = AnalyseNextNumber(third);
            if (thirdCode == -1)
            {
                return "分析第二次用户失败";
            }
            string end = web.Post(UnfreezeByAvatarQuickUnfreezePostUrl, string.Format("number={1}&st={0}", st, thirdCode));
            if (end == "{\"code\":\"1000\",\"msg\":\"check success\"}")
            {
                return "解封成功";
            }
            return "解封失败";
        }

        private static string GetUidFromNickname(string nickname)
        {
            WebAccessBase web = new WebAccessBase();
            string url = string.Format("http://m.weibo.cn/n/{0}", nickname);
            for (int i = 0; i < 5; i++)
            {
                var home = web.GetHTML(url);
                if (string.IsNullOrEmpty(home))
                    continue;
                return CnUnfreezeUidRegex.Match(home).Groups["uid"].Value;
            }
            return null;
        }

        private static int AnalyseNextNumber(string html)
        {
            try
            {
                dynamic json = DynamicJson.Parse(html);
                if (json.code == "1000")
                {
                    string avatarUrl = json.avatar;
                    int i = 1;
                    foreach (string nickname in json.nickname)
                    {
                        var uid = GetUidFromNickname(nickname);
                        if (!string.IsNullOrEmpty(uid))
                        {
                            if (avatarUrl.Contains(uid))
                                return i;
                        }
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                CNHttpWorkLogger.Error(ex.ToString(), ex);
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static string UnfreezeByPhone(WebAccessBase web)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region SinaWeiboCnSpider.Search

        /*
         * 爬取用户信息
         * 输入uid，输入用户信息
         * 作为爬博文的依据
         * 
         * 爬取uid的一页博文
         * 输入是uid,page，输出是文本
         * 输入一个cookies后，自己new爬虫
         * reffer使用http://m.weibo.cn/u/uid
         * 
         * 爬虫博文的一页评论
         * 输入是uid,mid,page，输出是文本
         * 输入一个cookies后，自己new爬虫
         * reffer使用http://m.weibo.cn/uid/mid 
         */

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string GetWeiboUser(string uid)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000,
                Reffer = null
            };
            //获取主页
            var url = string.Format("http://m.weibo.cn/u/{0}", uid);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取一页用户微博数据
        /// </summary>
        /// <param name="weiboUser">用户信息</param>
        /// <param name="page">微博页数</param>
        /// <returns></returns>
        public static string GetMblogs(CommonEntityLib.Entities.user.Entity weiboUser, int page)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000
            };
            var reffer = string.Format("http://m.weibo.cn/u/{0}", weiboUser.ID);
            web.Reffer = new Uri(reffer);
            var url = string.Format("http://m.weibo.cn/page/json?containerid={0}_-_WEIBO_SECOND_PROFILE_WEIBO&page={1}", weiboUser.IDStr, page);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 根据微博Url获取微博内容页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetSingleMblog(string url)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000,
                Reffer = null
            };

            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取关注页面内容
        /// </summary>
        /// <param name="weiboUser"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetFollowers(CommonEntityLib.Entities.user.Entity weiboUser, int page)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000
            };
            var reffer = string.Format("http://m.weibo.cn/u/{0}", weiboUser.ID);
            web.Reffer = new Uri(reffer);
            var url = string.Format("http://m.weibo.cn/page/json?containerid={0}_-_FOLLOWERS&page={1}", weiboUser.IDStr, page);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取一页微博的评论数据
        /// </summary>
        /// <param name="uid">用户uid</param>
        /// <param name="id">微博id</param>
        /// <param name="page">评论页数</param>
        /// <returns></returns>
        public static string GetComment(long uid, string id, int page)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000
            };
            var mid = id.IdToMid();
            web.Reffer = string.IsNullOrEmpty(mid) ? new Uri(string.Format("http://m.weibo.cn/{0}/{1}", uid, mid)) : null;
            var url = string.Format("http://m.weibo.cn/single/rcList?format=cards&id={0}&type=comment&hot=0&page={1}", id, page);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取一页微博的评论数量
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int GetCommentNum(long uid, string id)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000
            };
            var mid = id.IdToMid();
            //uid = 1454461772;
            //mid = "C4SFndfzN";
            var url = string.Format("http://m.weibo.cn/{0}/{1}", uid, mid);
            string html = web.GetHTML(url);
            string str = Regex.Match(html, "\"comments_count\":(.*?),").Groups[1].Value;
            return Int32.Parse(str);
        }

        /// <summary>
        /// 获取粉丝页面
        /// </summary>
        /// <param name="weiboUser"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetFans(CommonEntityLib.Entities.user.Entity weiboUser, int page)
        {
            WebAccessBase web = new WebAccessBase
            {
                TimeOut = 30000
            };
            var reffer = string.Format("http://m.weibo.cn/u/{0}", weiboUser.ID);
            web.Reffer = new Uri(reffer);
            //var url = string.Format("http://m.weibo.cn/page/json?containerid={0}_-_FANS&page={1}", weiboUser.StageId, page);
            var url = string.Format("http://m.weibo.cn/page/json?containerid={0}_-_FANS&page={1}", weiboUser.IDStr, page);

            return web.GetHTML(url);
        }

        #endregion

        #region SinaWeiboCnSpider.UserSpider

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static bool Exist(string uid)
        {
            WebAccessBase web = new WebAccessBase();
            for (int i = 0; i < 10; i++)
            {
                var home = web.GetHTML(string.Format("http://m.weibo.cn/u/{0}", uid));
                if (string.IsNullOrEmpty(home))
                {
                    continue;
                }
                if (home.Contains("<title>相关信息</title>") && home.Contains(@"\u8fd9\u91cc\u8fd8\u6ca1\u6709\u5185\u5bb9"))
                {
                    return false;
                }
                if (home.Contains(uid))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static CommonEntityLib.Entities.user.Entity GetWeiboUserEntity(string uid)
        {
            var homeStr = GetWeiboUser(uid);
            return AnalyseCnPage.AnalysisUserHome(homeStr);
        }

        #endregion

        #region SinaWeiboCnSpider.BowenSpider

        private static readonly Regex BowenSpiderHtmlRegex = new Regex("<[^>]+>");

        /// <summary>
        /// 根据UID获取第一页博文列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<CommonEntityLib.Entities.status.Entity> BowenSpiderExecute(string uid)
        {
            try
            {
                var weiboUser = AnalyseCnPage.AnalysisUserHome(GetWeiboUser(uid));

                if (weiboUser != null && weiboUser.StatusesCount > 0)
                {
                    //对于网络错误进行重试
                    //for (int i = 0; i < 3; i++)
                    //{
                    var pageStr = GetMblogs(weiboUser, 1);
                    var list = AnalyseCnPage.AnalysisWeiboPage(pageStr);
                    foreach (var mblog in list)
                    {
                        //去掉HTML标签
                        mblog.Text = BowenSpiderHtmlRegex.Replace(mblog.Text, "");
                    }
                    return list;
                    //}
                }
            }
            catch (Exception exception)
            {
                CNHttpWorkLogger.Error(string.Format("UID:{0}获取博文页面发生异常", uid), exception);
            }
            return new List<CommonEntityLib.Entities.status.Entity>();
        }

        #endregion

        #region SinaWeiboCnSpider.AttentionSpider

        /// <summary>
        /// 获取最后的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<long> AttentionSpiderExecute1(string uid)
        {
            List<long> result = new List<long>();

            var weiboUser = AnalyseCnPage.AnalysisUserHome(GetWeiboUser(uid));
            for (int page = weiboUser.FriendsCount / 10 + 1; page >= 1; page--)
            {
                //对于网络错误进行重试
                for (int i = 0; i < 3; i++)
                {
                    var pageStr = GetFollowers(weiboUser, page);
                    var pageList = AnalyseCnPage.AnalysisFollowers(pageStr);
                    if (result.Distinct().ToList().Count > 10)
                    {
                        return result.Distinct().ToList();
                    }
                    if (pageList != null)
                    {
                        result.AddRange(pageList);
                        break;
                    }
                    CNHttpWorkLogger.Info("用户{0}第{1}页关注获取失败", uid, page);
                }
            }
            //去重
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 获取20个fans
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<long> Get20Fans(string uid)
        {
            List<long> result = new List<long>();

            var weiboUser = AnalyseCnPage.AnalysisUserHome(GetWeiboUser(uid));
            for (int page = 1; page <= weiboUser.FriendsCount / 10 + 1; page++)
            {
                //对于网络错误进行重试
                for (int i = 0; i < 3; i++)
                {
                    var pageStr = GetFans(weiboUser, page);
                    var pageList = AnalyseCnPage.AnalysisFans(pageStr);
                    if (result.Distinct().ToList().Count > 10)
                    {
                        return result.Distinct().ToList();
                    }
                    if (pageList != null)
                    {
                        result.AddRange(pageList);
                        break;
                    }
                    CNHttpWorkLogger.Info("用户{0}第{1}页粉丝获取失败", uid, page);
                }
            }
            //去重
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 获取最前的关注UID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<long> AttentionSpiderExecute(string uid)
        {
            List<long> result = new List<long>();

            var weiboUser = AnalyseCnPage.AnalysisUserHome(GetWeiboUser(uid));
            for (int page = 1; page <= weiboUser.FriendsCount / 10 + 1; page++)
            {
                //对于网络错误进行重试
                for (int i = 0; i < 3; i++)
                {
                    var pageStr = GetFollowers(weiboUser, page);
                    var pageList = AnalyseCnPage.AnalysisFollowers(pageStr);
                    if (pageList != null)
                    {
                        result.AddRange(pageList);
                        break;
                    }
                    CNHttpWorkLogger.Info("用户{0}第{1}页关注获取失败", uid, page);
                }
            }
            //去重
            return result.Distinct().ToList();
        }

        /// <summary>
        /// 并行获取关注的UID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<long> AttentionSpiderParallelExecute(string uid)
        {
            ConcurrentBag<long> concurrentBag = new ConcurrentBag<long>();

            var weiboUser = AnalyseCnPage.AnalysisUserHome(GetWeiboUser(uid));
            var totalPage = weiboUser.FriendsCount / 10 + 1;

            Action<int> action = page =>
            {
                //对于网络错误进行重试
                for (int i = 0; i < 3; i++)
                {
                    var pageStr = GetFollowers(weiboUser, page);
                    var pageList = AnalyseCnPage.AnalysisFollowers(pageStr);
                    if (pageList != null)
                    {
                        foreach (long id in pageList)
                        {
                            concurrentBag.Add(id);
                        }
                        break;
                    }
                    CNHttpWorkLogger.Info("用户{0}第{1}页关注获取失败", uid, page);
                }
            };

            ParallelOptions po = new ParallelOptions { MaxDegreeOfParallelism = 32 };
            Parallel.For(1, totalPage, po, action);
            return concurrentBag.Distinct().ToList();
        }

        #endregion

        #region weiboHttpSearch

        /// <summary>
        /// 用户信息搜索
        /// </summary>
        /// <param name="factor1">类型：只搜公司 1 只搜学校 2 只搜标签 3</param>
        /// <param name="factor2">性别：男性 4  女性 5</param>
        /// <param name="factor3">用户类型：机构认证用户 6 个人认证用户 7 普通用户 8</param>
        /// <param name="factor4">年龄段：18以下_9 19到22_10 23到29_11 30到39_12 40以上_13</param>
        /// <param name="queryWord">查询字符串</param>
        /// <returns>符合条件的UserID列表</returns>
        public static List<string> GetWeiboHttpSearchUserIDs(int factor1, int factor2, int factor3, int factor4, string queryWord)
        {
            string url = GetWeiboHttpSearchUrl(factor1, factor2, factor3, factor4, queryWord);
            return GetWeiboHttpSearchUserIDs(url);
        }

        private static string GetWeiboHttpSearchUrl(int factor1, int factor2, int factor3, int factor4, string queryWord)
        {
            string searchUrl;

            string url = string.Empty;
            string isv = string.Empty;
            string birth = string.Empty;
            string gender = string.Empty;

            //男性 女性
            if (factor2 != 0)
            {
                if (factor2 == 4)
                {
                    gender = Convert.ToString(1);
                }
                if (factor2 == 5)
                {
                    gender = Convert.ToString(2);
                }
            }

            //用户类型
            if (factor3 != 0)
            {
                if (factor3 == 6)
                {
                    isv = Convert.ToString(3);
                }
                if (factor3 == 7)
                {
                    isv = Convert.ToString(2);
                }
                if (factor3 == 8)
                {
                    isv = Convert.ToString(0);
                }
            }

            //年龄段
            if (factor4 != 0)
            {
                if (factor4 == 9)
                {
                    birth = "&sbirth=" + (DateTime.Now.Year - 18);
                }
                if (factor4 == 10)
                {
                    birth = "&sbirth=" + (DateTime.Now.Year - 22) + "&ebirth" + (DateTime.Now.Year - 19);
                }
                if (factor4 == 11)
                {
                    birth = "&sbirth=" + (DateTime.Now.Year - 29) + "&ebirth" + (DateTime.Now.Year - 23);
                }
                if (factor4 == 12)
                {
                    birth = "&sbirth=" + (DateTime.Now.Year - 39) + "&ebirth" + (DateTime.Now.Year - 30);
                }
                if (factor4 == 13)
                {
                    birth = "&ebirth=" + (DateTime.Now.Year - 40) + "&sebirth=1901";
                }
            }

            if (factor1 == 0 && factor2 == 0 && factor3 == 0 & factor4 == 0)
            {
                url = "type=3&q=" + queryWord;
            }

            if (factor2 == 0 && factor3 == 0 && factor4 == 0)
            {
                if (factor1 == 1)
                {
                    url = "type=3&q=&comp=" + queryWord + "&specfilter=1&log_type=" + factor1;
                }
                if (factor1 == 2)
                {
                    url = "type=3&q=&scho=" + queryWord + "&specfilter=1&log_type=" + factor1;
                }
                if (factor1 == 3)
                {
                    url = "type=3&q=&tags=" + queryWord + "&specfilter=1&log_type=" + factor1;
                }
            }


            if (factor1 == 0)
            {
                if (factor2 != 0 && factor3 == 0 && factor4 == 0)
                {
                    url = "type=3&q=" + queryWord + "&gender=" + gender + "&specfilter=1&log_type=" + factor2;
                }

                if (factor3 != 0 && factor2 == 0 && factor4 == 0)
                {
                    url = "type=3&q=" + queryWord + "&isv=" + isv + "&specfilter=1&log_type=" + factor3;
                }

                if (factor4 != 0 && factor2 == 0 && factor3 == 0)
                {
                    url = "type=3&q=" + queryWord + "&specfilter=1&log_type=" + factor4 + birth;
                }

                if (factor2 != 0 && factor3 != 0 && factor4 == 0)
                {
                    url = "type=3&q=" + queryWord + "&gender=" + gender + "&specfilter=2&log_type=" + factor2 + "&isv=" + isv + "&log_type=" + factor3;
                }
                if (factor2 != 0 && factor3 == 0 && factor4 != 0)
                {
                    url = "type=3&q=" + queryWord + "&gender=" + gender + "&specfilter=2&log_type=" + factor2 + birth + "&log_type=" + factor4;
                }
                if (factor2 == 0 && factor3 != 0 && factor4 != 0)
                {
                    url = "type=3&q=" + queryWord + "&isv=" + isv + "&specfilter=2&log_type=" + factor3 + birth + "&log_type=" + factor4;
                }
                if (factor2 != 0 && factor3 != 0 && factor4 != 0)
                {
                    url = "type=3&q=" + queryWord + "&gender=" + gender + "&specfilter=3&log_type=" + factor2 + "&isv=" + isv + "&log_type=" + factor3 + birth + "&log_type=" + factor4;
                }
            }

            if (factor1 == 0 && factor2 == 0 && factor3 == 0 & factor4 == 0)
            {
                url = "type=3&q=" + queryWord;
                searchUrl = "http://m.weibo.cn/main/pages/index?containerid=100103" + HttpUtility.UrlEncode(url);
            }
            else
            {
                searchUrl = "http://m.weibo.cn/p/index?containerid=100103" + HttpUtility.UrlEncode(url);
            }
            return searchUrl;
        }

        private static List<string> GetWeiboHttpSearchUserIDs(string searchUrl)
        {
            List<string> list = new List<string>();

            WebAccessBase web = new WebAccessBase("Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            string code = web.GetHTML(searchUrl);

            const string regex1 = "(?<=\"id\":).*?(?=,\"screen_name\")";

            Regex reg1 = new Regex(regex1);
            MatchCollection idCollection = reg1.Matches(code);
            for (int i = 0; i < idCollection.Count; i++)
            {
                list.Add(idCollection[i].Value);
            }

            if (code.Contains("maxPage"))
            {
                const string regex2 = "(?<=\"maxPage\":).*?(?=,\"page\")";
                Regex reg2 = new Regex(regex2);
                Match mPageCount = reg2.Match(code);
                int totalPageCount = int.Parse(mPageCount.ToString());

                const string regex3 = "(?<=\"page\":1,\"url\":\").*?(?=\",\"previous_cursor\")";
                Regex reg3 = new Regex(regex3);
                Match mMoreUserUrl = reg3.Match(code);

                string moreUserUrl = ("http://m.weibo.cn" + mMoreUserUrl).Replace("\\", "");

                GetMoreWeiboHttpSearchUserIDs(moreUserUrl, totalPageCount, 2, ref list);
            }
            return list;
        }

        private static void GetMoreWeiboHttpSearchUserIDs(string moreUserUrl, int pageCount, int i, ref List<string> idList)
        {
            while (true)
            {
                if (i <= pageCount)
                {
                    string searchUrl = moreUserUrl + "&page=" + i;
                    WebAccessBase web = new WebAccessBase("Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

                    string code = web.GetHTML(searchUrl);

                    const string regex = "(?<=\"id\":).*?(?=,\"screen_name\")";
                    Regex reg = new Regex(regex);
                    MatchCollection idCollection = reg.Matches(code);

                    List<string> tempList = new List<string>();
                    for (int j = 0; j < idCollection.Count; j++)
                    {
                        tempList.Add(idCollection[j].Value);
                    }
                    idList = idList.Union(tempList).ToList();
                    i++;
                    continue;
                }
                break;
            }
        }

        #endregion
    }
}
