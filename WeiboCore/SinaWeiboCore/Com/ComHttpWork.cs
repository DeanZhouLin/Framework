using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommonEntityLib.Entities.comment;
using CommonEntityLib.Entities.message;
using CommonEntityLib.Entities.tag;
using HtmlAgilityPack;
using NLog;
using SinaWeiboCore.CN;
using SinaWeiboCore.ReffDll;
using Collection = CommonEntityLib.Entities.user.Collection;

namespace SinaWeiboCore.Com
{
    /// <summary>
    /// Com方式的HttpWork类
    /// </summary>
    public class ComHttpWork : IHttpWork
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly Logger ComHttpWorkLogger = LogManager.GetLogger("ComHttpWork");

        /// <summary>
        /// HttpWork类实例
        /// </summary>
        public static readonly ComHttpWork Instance = new ComHttpWork();

        /// <summary>
        /// 构造函数
        /// </summary>
        private ComHttpWork()
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

            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
            string picsPid = picPidOrPaths.GetPicsPid(webLogin);

            return AddMblog(login, text, picsPid);
        }

        /// <summary>
        /// 批量上传图片到新浪
        /// </summary>
        /// <param name="webLogin">当前登陆对象</param>
        /// <param name="picContents">图片Byte[]集合</param>
        /// <returns>图片Pid列表</returns>
        public List<string> AddPicture(IWeiboLogin webLogin, params byte[][] picContents)
        {
            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
            login.Web.Encode = Encoding.UTF8;
            string html = login.Web.GetHTML("http://weibo.com/");
            if (string.IsNullOrEmpty(html))
            {
                ComHttpWorkLogger.Info(string.Format("上传图片失败\r\n"));
                throw new Exception("上传图片失败 http://weibo.com/ 返回空");
            }
            var onick = onickReg.Match(html).Groups["onick"].Value;
            var oid = oidReg.Match(html).Groups["oid"].Value;

            List<string> res = new List<string>();

            foreach (var pic in picContents)
            {
                var pichtml = login.Web.UploadImage(pic, string.Format("http://picupload.service.weibo.com/interface/pic_upload.php?app=miniblog&data=1&url=weibo.com/u/{0}&markpos=1&logo=1&nick={1}&marks=1&mime=image/jpeg&ct={2}", onick, oid, new Random(Guid.NewGuid().GetHashCode()).NextDouble()))
                              ?? "";
                if (string.IsNullOrEmpty(pichtml))
                {
                    ComHttpWorkLogger.Info("图片pid获取失败\r\n{0}图片长度：\r\n{1}", pichtml, pic.Length);
                }
                else
                {
                    res.Add(pidReg.Match(pichtml).Groups["pid"].Value);
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
            byte[][] pics = picPaths.Select(File.ReadAllBytes).ToArray();
            return AddPicture(webLogin, pics);
        }

        /// <summary>
        /// 加关注
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">关注用户uid</param>
        /// <returns>结果 空字符串表示成功</returns>
        public string FriendCreate(IWeiboLogin webLogin, string uid)
        {
            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
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
            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
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
            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
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
            ComWeiboLogin login = webLogin.GetWeiboLogin<ComWeiboLogin>();
            return DelMblog(login.Web, url, mid);
        }

        public string UploadAvatar(IWeiboLogin webLogin, string cloneUid)
        {
            throw new NotImplementedException();
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
            ComHttpWorkLogger.Warn("Com 未实现AttitudesDestroy");
            return "";
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
            ComHttpWorkLogger.Warn("Com 未实现SaveInformation");
            return "";
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
            ComHttpWorkLogger.Warn("Com 未实现SendDirectMessages");
            return "";
        }

        /// <summary>
        /// 收发给自己的私信
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid">账号自己的Uid</param>
        /// <returns>私信列表</returns>
        public List<CnDirectMessagesEntity> RecevieDirectMessages(IWeiboLogin webLogin, string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现RecevieDirectMessages");
            return new List<CnDirectMessagesEntity>();
        }

        /// <summary>
        /// 收所有的私信会话
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CnDirectMessagesEntity> RecevieAllDirectMessages(IWeiboLogin webLogin, string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现RecevieAllDirectMessages");
            return new List<CnDirectMessagesEntity>();
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
            ComHttpWorkLogger.Warn("Com 未实现SendReplyComment");
            return "";
        }

        /// <summary>
        /// 收发给自己的评论
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<Entity> ReceiveComment(IWeiboLogin webLogin, string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现ReceiveComment");
            return new List<Entity>();
        }

        /// <summary>
        /// 根据uid获取用户信息
        /// </summary>
        /// <param name="webLogin">登陆对象</param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.user.Entity GetUserEntity(IWeiboLogin webLogin, string uid)
        {
            IWeiboLogin cnWeiboLogin = PlatformType.CN.GetWeiboLogin();
            cnWeiboLogin.Web.Cookie = webLogin.Web.Cookie;

            if (!CNHttpWork.UserExist(cnWeiboLogin.Web, uid))
            {
                return null;
            }

            CommonEntityLib.Entities.user.Entity res = CNHttpWork.GetUserEntity(cnWeiboLogin.Web, uid);
            if (res == null)
            {
                return null;
            }

            string url = "http://weibo.com/" + uid + "/info";
            string html = webLogin.Web.GetHTML(url);
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var temHtmlList = new Dictionary<string, string>();
            if (html.Contains("<script>FM"))
            {
                var regex = new Regex(@"FM.view\((?<json>.*?)\)</script>");
                if (regex.IsMatch(html))
                {
                    foreach (Match match in regex.Matches(html))
                    {
                        string jsonStr = match.Groups["json"].Value;
                        if (!jsonStr.Contains("等级信息") && !jsonStr.Contains("基本信息") && !jsonStr.Contains("他的主页") && !jsonStr.Contains("Pl_Official_Headerv6__1"))
                        {
                            continue;
                        }

                        var json = DynamicJson.Parse(jsonStr);
                        string domid = json.domid;
                        try
                        {
                            string ht = json.html;
                            for (var i = (char)1; i < (char)32; i++)
                            {
                                ht = ht.Replace(i.ToString(CultureInfo.InvariantCulture), string.Empty);
                            }
                            ht = ht.Replace("\x7F", string.Empty);

                            temHtmlList.Add(domid, ht);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            foreach (var thl in temHtmlList)
            {
                if (thl.Key.Contains("Pl_Official_Headerv6__1"))
                {
                    res.Verified = thl.Value.Contains("verified.weibo.com");
                    if (res.Verified)
                    {
                        HtmlDocument document = new HtmlDocument();
                        document.LoadHtml(thl.Value);
                        var root = document.DocumentNode;

                        res.VerifiedReason = root.SelectSingleNode("div/div/div[@class='shadow  S_shadow']/div[@class='pf_photo']/a/em")
                                .Attributes["title"].Value;
                    }
                    continue;
                }

                if (thl.Value.Contains("等级信息"))
                {
                    // 等级
                    // <span>Lv.2</span>
                    string lv = Regex.Match(thl.Value, "<span>Lv\\.(?<lv>\\d*)</span>").Groups["lv"].Value;
                    int uRank;
                    if (!string.IsNullOrEmpty(lv) && int.TryParse(lv, out uRank))
                    {
                        res.Urank = uRank;
                    }
                    continue;
                }

                if (thl.Value.Contains("他的主页"))
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(thl.Value);
                    var root = document.DocumentNode;
                    var aTag = root.SelectSingleNode("div/div/table/tr/td/a");
                    if (aTag != null)
                    {
                        res.ProfileUrl = string.Format("http://weibo.com{0}", aTag.Attributes["href"].Value);
                    }
                }

                if (thl.Value.Contains("基本信息"))
                {
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(thl.Value);
                    var root = document.DocumentNode;
                    HtmlNodeCollection categoryNodeList = root.SelectNodes("//ul[@class='clearfix']/li");

                    if (categoryNodeList == null)
                    {
                        continue;
                    }

                    foreach (HtmlNode htmlNode in categoryNodeList)
                    {
                        HtmlNodeCollection spans = htmlNode.SelectNodes("span");
                        if (spans.Count != 2)
                        {
                            continue;
                        }

                        string txt = spans[0].InnerText;
                        string val = spans[1].InnerText;

                        if (txt.Contains("博客"))
                        {
                            res.Url = val;
                            continue;
                        }
                        if (txt.Contains("个性域名"))
                        {
                            res.Domain = val;
                            continue;
                        }
                        if (txt.Contains("注册时间"))
                        {
                            res.CreatedAt = val;
                            continue;
                        }
                        if (txt.Contains("所在地"))
                        {
                            res.Location = val;
                            if (!string.IsNullOrEmpty(res.Location) && res.Location.Split(' ').Count() >= 2)
                            {
                                string provice = res.Location.Split(' ')[0].Trim();
                                string city = res.Location.Split(' ')[1].Trim();
                                res.Province = ProvinceToCode(provice);
                                res.City = CityToCode(city);
                            }
                            continue;
                        }
                        if (txt.Contains("标签"))
                        {
                            HtmlNodeCollection tags = spans[1].SelectNodes("a");
                            if (tags != null)
                            {
                                res.Remark = string.Join(",", tags.Select(c => c.InnerText));
                            }
                        }
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.user.Entity GetUserEntity(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetUserEntity uid");
            return new CommonEntityLib.Entities.user.Entity();
        }

        public Collection GetUserEntityColloction(string appKey, string uids)
        {
            throw new NotImplementedException();
        }

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
            ComHttpWorkLogger.Warn("Com 未实现GetMblogs");
            return new List<CommonEntityLib.Entities.status.Entity>();
        }

        /// <summary>
        /// 根据UID获取第一页博文列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetMblogs uid");
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
        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(WebAccessBase web, string uid, string mid, int page)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetMblogs web uid mid page");
            return new List<CommonEntityLib.Entities.status.Entity>();
        }

        /// <summary>
        /// 根据uid判断用户是否存在
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool UserExist(IWeiboLogin webLogin, string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现UserExist");
            return false;
        }

        /// <summary>
        /// 检测用户是否存在
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool UserExist(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现UserExist uid");
            return false;
        }

        /// <summary>
        /// 获取新增粉丝的第一页
        /// </summary>
        /// <param name="webLogin"></param>
        public List<string> GetNewFans(IWeiboLogin webLogin)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetNewFans");
            return new List<string>();
        }

        /// <summary>
        /// 根据m.weibo.cn博文地址获取博文信息
        /// </summary>
        /// <param name="webLogin"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public CommonEntityLib.Entities.status.Entity GetMblog(IWeiboLogin webLogin, string url)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetMblog");
            return new CommonEntityLib.Entities.status.Entity();
        }

        /// <summary>
        /// 获取最后的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>最后的关注UID列表</returns>
        public List<long> GetLastAttentionUIDs(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetLastAttentionUIDs");
            return new List<long>();
        }

        /// <summary>
        /// 获取最前的关注UID列表。
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>获取最前的关注UID列表</returns>
        public List<long> GetPrewAttentionUIDs(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetPrewAttentionUIDs");
            return new List<long>();
        }

        /// <summary>
        /// 获取20个fans
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>20个fans</returns>
        public List<long> Get20Fans(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现Get20Fans");
            return new List<long>();
        }

        /// <summary>
        /// 并行获取关注的UID列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public List<long> GetAttentionUIDsAsParallel(string uid)
        {
            ComHttpWorkLogger.Warn("Com 未实现GetAttentionUIDsAsParallel");
            return new List<long>();
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
        public List<string> GetWeiboHttpSearchUserIDs(int factor1, int factor2, int factor3, int factor4, string queryWord)
        {
            return CNHttpWork.GetWeiboHttpSearchUserIDs(factor1, factor2, factor3, factor4, queryWord);
        }

        #region SinaWeibo.HttpWork

        static readonly Regex pidReg = new Regex("\"pid\":\"(?<pid>.*?)\"");
        static readonly Regex onickReg = new Regex("[[]\'onick\'[]]=\'(?<onick>.*?)\'");
        static readonly Regex oidReg = new Regex("[[]\'oid\'[]]=\'(?<oid>.*?)\'");

        static readonly Regex OidRegex = new Regex(@"\$CONFIG\['oid'\]='(?<oid>.*?)';");
        static readonly Regex LocationRegex = new Regex(@"\$CONFIG\['location'\]='(?<location>.*?)';");
        static readonly Regex NickRegex = new Regex(@"\$CONFIG\['onick'\]='(?<onick>.*?)';");

        private static readonly string ProvinceAndCity = File.ReadAllText("provinceandcity.txt");

        #region 发布 SendMsg | AddMblog

        /// <summary>
        /// 只发布文字信息内容
        /// </summary>
        /// <param name="login">登录信息</param>
        /// <param name="msg">需要发布的文本内容</param>
        /// <returns></returns>
        public static string SendMsg(ComWeiboLogin login, string msg)
        {
            if (login.Web == null)
            {
                throw new Exception("Web为空");
            }
            if (Encoding.GetEncoding("GBK").GetByteCount(msg) * 2 > 280)
            {
                throw new Exception("文本内容超出280字节");
            }
            login.Web.Encode = Encoding.UTF8;
            //login.Web.GetHTML("http://weibo.com/");
            login.Web.Reffer = new Uri("http://weibo.com/");
            var url = string.Format("http://weibo.com/aj/mblog/add?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData = string.Format("location=v6_content_home&appkey=&style_type=1&pic_id=&text={0}&pdetail=&rank=0&rankid=&module=stissue&pub_type=dialog&_t=0", msg);
            var htmlMsg = login.Web.Post(url, postData);
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n{0}", htmlMsg));
            return string.Format("发表失败");

        }

        /// <summary>
        /// 发布文字信息与多张图片内容
        /// </summary>
        /// <param name="login">登录信息</param>
        /// <param name="msg">信息内容</param>
        /// <param name="picPids">图片pid数组</param>
        /// <returns></returns>
        public static string AddMblog(ComWeiboLogin login, string msg, string picPids)
        {
            if (Encoding.GetEncoding("GBK").GetByteCount(msg) * 2 > 280)
            {
                throw new Exception("文本内容超出280字节");
            }
            if (picPids.Split(' ').Count() > 9)
            {
                throw new Exception("图片超出9张");
            }
            login.Web.Reffer = new Uri("http://weibo.com/");
            login.Web.Encode = Encoding.UTF8;
            var url = string.Format("http://weibo.com/aj/mblog/add?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData = string.Format("location=v6_content_home&appkey=&style_type=1&pic_id={0}&text={1}&pdetail=&rank=0&rankid=&module=stissue&pub_type=dialog&_t=0", picPids, msg);
            var htmlMsg = login.Web.Post(url, postData);
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n{0}", htmlMsg));
            return string.Format("发表失败");
        }

        /// <summary>
        /// 发布文字信息与多张图片内容
        /// </summary>
        /// <param name="login">登录信息</param>
        /// <param name="picPaths">图片路径数组</param>
        /// <param name="msg">需要发布的文本内容</param>
        /// <returns></returns>
        public static string SendMsg(ComWeiboLogin login, string[] picPaths, string msg)
        {
            if (Encoding.GetEncoding("GBK").GetByteCount(msg) * 2 > 280)
            {
                throw new Exception("文本内容超出280字节");
            }
            if (picPaths.Length > 9)
            {
                throw new Exception("图片超出9张");
            }
            login.Web.Encode = Encoding.UTF8;
            string html = login.Web.GetHTML("http://weibo.com/");
            if (string.IsNullOrEmpty(html))
            {
                ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n"));
                return string.Format("发表失败");
            }
            var onick = onickReg.Match(html).Groups["onick"].Value;
            var oid = oidReg.Match(html).Groups["oid"].Value;
            var pid = string.Empty;
            for (int i = 0; i < picPaths.Length; i++)
            {
                string picPath = picPaths[i];
                if (picPath.Substring(picPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "jpg" && picPath.Substring(picPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "png")
                {
                    throw new Exception(string.Format("图片文件扩展名错误（{0}）", picPath));
                }
                var pic = File.ReadAllBytes(picPath);
                var pichtml = login.Web.UploadImage(pic, string.Format("http://picupload.service.weibo.com/interface/pic_upload.php?app=miniblog&data=1&url=weibo.com/u/{0}&markpos=1&logo=1&nick={1}&marks=1&mime=image/jpeg&ct={2}", onick, oid, new Random(Guid.NewGuid().GetHashCode()).NextDouble()))
                    ?? "";
                if (string.IsNullOrEmpty(pichtml))
                {
                    ComHttpWorkLogger.Info("图片pid获取失败\r\n{0}图片路径：\r\n{1}", pichtml, picPath);
                }
                else
                {
                    if (i < 1)
                    {
                        pid += pidReg.Match(pichtml).Groups["pid"].Value;
                    }
                    else
                    {
                        pid += " " + pidReg.Match(pichtml).Groups["pid"].Value;
                    }
                }

            }
            var url = string.Format("http://weibo.com/aj/mblog/add?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData = string.Format("location=v6_content_home&appkey=&style_type=1&pic_id={0}&text={1}&pdetail=&rank=0&rankid=&module=stissue&pub_type=dialog&_t=0", pid, msg);
            var htmlMsg = login.Web.Post(url, postData);
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n{0}", htmlMsg));
            return string.Format("发表失败");
        }

        /// <summary>
        /// 发布文字信息与单张图片内容
        /// </summary>
        /// <param name="login">登录信息</param>
        /// <param name="picPath">图片路径</param>
        /// <param name="msg">需要发布的文本内容</param>
        /// <returns></returns>
        public static string SendMsg(ComWeiboLogin login, string picPath, string msg)
        {
            if (Encoding.GetEncoding("GBK").GetByteCount(msg) * 2 > 280)
            {
                throw new Exception("文本内容超出280字节");
            }
            login.Web.Encode = Encoding.UTF8;
            string html = login.Web.GetHTML("http://weibo.com/");
            if (string.IsNullOrEmpty(html))
            {
                ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n"));
                return string.Format("发表失败");
            }
            if (picPath.Substring(picPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "jpg" && picPath.Substring(picPath.LastIndexOf(".", StringComparison.Ordinal) + 1).ToLower() != "png")
            {
                throw new Exception(string.Format("图片文件扩展名错误（{0}）", picPath));
            }
            var onick = onickReg.Match(html).Groups["onick"].Value;
            var oid = oidReg.Match(html).Groups["oid"].Value;
            var pic = File.ReadAllBytes(picPath);
            var pichtml = login.Web.UploadImage(pic, string.Format("http://picupload.service.weibo.com/interface/pic_upload.php?app=miniblog&data=1&url=weibo.com/u/{0}&markpos=1&logo=1&nick={1}&marks=1&mime=image/jpeg&ct={2}", onick, oid, new Random(Guid.NewGuid().GetHashCode()).NextDouble()))
                ?? "";
            if (string.IsNullOrEmpty(pichtml))
            {
                ComHttpWorkLogger.Info("图片pid获取失败\r\n{0}图片路径：\r\n{1}", pichtml, picPath);
            }
            var pid = pidReg.Match(pichtml).Groups["pid"].Value;
            var url = string.Format("http://weibo.com/aj/mblog/add?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData = string.Format("location=v6_content_home&appkey=&style_type=1&pic_id={0}&text={1}&pdetail=&rank=0&rankid=&module=stissue&pub_type=dialog&_t=0", pid, msg);
            var htmlMsg = login.Web.Post(url, postData);
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("微博发表失败\r\n{0}", htmlMsg));
            return string.Format("发表失败");
        }

        /// <summary>
        /// 发表微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="text">需要发布的文本内容</param>
        /// <param name="pic"></param>
        /// <param name="appkey"></param>
        /// <returns></returns>
        public static string AddMblog(WebAccessBase web, string text, string pic = null, string appkey = null)
        {
            if (web == null)
            {
                throw new ArgumentNullException("web");
            }
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text", "微博内容不能为空");
            }
            web.Encode = Encoding.UTF8;
            web.Reffer = new Uri("http://weibo.com/");
            var url = string.Format("http://weibo.com/aj/mblog/add?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData =
                string.Format(
                    "location=v6_content_home&appkey={2}&style_type=1&pic_id={1}&text={0}&pdetail=&rank=0&rankid=&module=stissue&pub_type=dialog&_t=0",
                    text, pic, appkey);
            var htmlMsg = web.PostWithHeaders(url, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("发微博失败\r\n{0}", htmlMsg));
            return string.Format("发微博失败");
        }

        /// <summary>
        /// 删除微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url"></param>
        /// <param name="mid"></param>
        /// <returns></returns>
        public static string DelMblog(WebAccessBase web, string url, string mid)
        {
            web.Reffer = new Uri("http://weibo.com/");
            var weiboStr = web.GetHTML(url);
            if (string.IsNullOrEmpty(weiboStr))
            {
                ComHttpWorkLogger.Info("访问微博{0}页面出错", url);
                return "网络错误 删除微博";
            }
            if (weiboStr.Contains("<script>parent.window.location=\"http://weibo.com/sorry?pagenotfound\"</script>"))
            {
                return "微博不存在";
            }
            const string posturl = "http://weibo.com/aj/mblog/del?ajwvr=6";
            var postData = "mid=" + mid;
            var htmlMsg = web.PostWithHeaders(posturl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            if (!string.IsNullOrEmpty(htmlMsg) && htmlMsg.Contains("\"code\":\"100000\""))
            {
                return "";
            }
            ComHttpWorkLogger.Info(string.Format("删除微博失败\r\n{0}", htmlMsg));
            return string.Format("删除微博失败");
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
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(string.Format("http://weibo.com/u/{0}", uid));
            if (string.IsNullOrEmpty(home))
            {
                return string.Format("访问关注对象{0}页面出错", uid);
            }
            if (home.Contains("<title>404错误</title>"))
            {
                return "工作对象被封";
            }
            var oid = OidRegex.Match(home).Groups["oid"].Value;
            var location = LocationRegex.Match(home).Groups["location"].Value;
            var nick = NickRegex.Match(home).Groups["onick"].Value;
            if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(nick))
            {
                ComHttpWorkLogger.Info("分析关注对象{0}页面出错\r\n{1}", uid, home);
                return string.Format("分析关注对象{0}页面出错", uid);
            }
            if (uid != oid)
            {
                ComHttpWorkLogger.Info("分析关注对象{0}页面UID出错\r\n{1}", uid, home);
                return string.Format("分析关注对象{0}页面UID出错", uid);
            }
            var postUrl = string.Format("http://weibo.com/aj/f/followed?ajwvr=6&__rnd={0}", CommonExtension.GetTime());
            var postData =
                string.Format(
                    "uid={0}&objectid=&f=1&extra=&refer_sort=&refer_flag=&location={1}&oid={2}&wforce=1&nogroup=false&fnick={3}&_t=0",
                    uid, location, oid, nick);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //关注结果分析
            if (string.IsNullOrEmpty(postHtml))
            {
                return "关注失败，返回空";
            }
            try
            {
                dynamic postResult = DynamicJson.Parse(postHtml);
                return postResult.code == "100000" ? "" : string.Format("关注失败，{0}", postResult.msg);
            }
            catch (Exception)
            {
                ComHttpWorkLogger.Info(string.Format("关注失败\r\n{0}", postHtml));
                return "关注失败，分析失败";
            }
        }

        /// <summary>
        /// 取消关注
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static string FriendDestroy(WebAccessBase web, string uid)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(uid))
                throw new ArgumentNullException("uid");
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(string.Format("http://weibo.com/u/{0}", uid));
            if (string.IsNullOrEmpty(home))
            {
                return string.Format("访问关注对象{0}页面出错", uid);
            }
            if (home.Contains("<title>404错误</title>"))
            {
                return "工作对象被封";
            }
            var oid = OidRegex.Match(home).Groups["oid"].Value;
            var location = LocationRegex.Match(home).Groups["location"].Value;
            var nick = NickRegex.Match(home).Groups["onick"].Value;
            if (string.IsNullOrEmpty(oid) || string.IsNullOrEmpty(location) || string.IsNullOrEmpty(nick))
            {
                ComHttpWorkLogger.Info("分析关注对象{0}页面出错\r\n{1}", uid, home);
                return string.Format("分析关注对象{0}页面出错", uid);
            }
            if (uid != oid)
            {
                ComHttpWorkLogger.Info("分析关注对象{0}页面UID出错\r\n{1}", uid, home);
                return string.Format("分析关注对象{0}页面UID出错", uid);
            }
            var postUrl = string.Format("http://weibo.com/aj/f/unfollow?ajwvr=6");
            var postData =
                string.Format(
                    "uid={0}&objectid=&f=1&extra=&refer_sort=&refer_flag=&location={1}&oid={2}&wforce=1&nogroup=false&fnick={3}",
                    uid, location, oid, nick);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //取消关注结果分析
            if (string.IsNullOrEmpty(postHtml))
            {
                return "取消关注失败，返回空";
            }
            try
            {
                dynamic postResult = DynamicJson.Parse(postHtml);
                return postResult.code == "100000" ? "" : string.Format("取消关注失败，{0}", postResult.msg);
            }
            catch (Exception)
            {
                ComHttpWorkLogger.Info(string.Format("取消关注失败\r\n{0}", postHtml));
                return "取消关注失败，分析失败";
            }
        }

        #endregion

        #region 点赞 AttitudesCreate

        /// <summary>
        /// 点赞
        /// </summary>
        /// <param name="web"></param>
        /// <param name="weiboUrl"></param>
        /// <param name="weiboId"></param>
        /// <returns></returns>
        public static string AttitudesCreate(WebAccessBase web, string weiboUrl, string weiboId)
        {
            if (web == null)
                throw new ArgumentNullException("web");
            if (string.IsNullOrEmpty(weiboUrl))
                throw new ArgumentNullException("weiboUrl");
            if (string.IsNullOrEmpty(weiboId))
                throw new ArgumentNullException("weiboId");
            web.Encode = Encoding.UTF8;
            web.Reffer = null;
            var home = web.GetHTML(weiboUrl);
            if (string.IsNullOrEmpty(home))
            {
                return string.Format("访问微博{0}页面出错", weiboUrl);
            }
            if (home.Contains("<title>404错误</title>"))
            {
                return "工作对象被封";
            }
            var location = LocationRegex.Match(home).Groups["location"].Value;
            if (string.IsNullOrEmpty(location))
            {
                return string.Format("分析微博{0}页面出错", weiboUrl);
            }
            var postUrl = string.Format("http://weibo.com/aj/v6/like/add?ajwvr=6");
            var postData =
                string.Format(
                    "version=mini&qid=heart&mid={0}&loc=profile&location={1}",
                    weiboId, location);
            var postHtml = web.PostWithHeaders(postUrl, postData, new[] { "X-Requested-With: XMLHttpRequest" });
            //点赞结果分析
            if (string.IsNullOrEmpty(postHtml))
            {
                return "点赞失败，返回空";
            }
            try
            {
                dynamic postResult = DynamicJson.Parse(postHtml);
                return postResult.code == "100000" ? "" : string.Format("点赞失败，{0}", postResult.msg);
            }
            catch (Exception)
            {
                ComHttpWorkLogger.Info(string.Format("点赞失败\r\n{0}", postHtml));
                return "点赞失败，分析失败";
            }
        }

        #endregion

        private static string ProvinceToCode(string province)
        {
            string pattern = string.Format("<province id=\"(?<code>.*?)\" name=\"{0}\">", province);
            return Regex.Match(ProvinceAndCity, pattern).Groups["code"].Value;
        }

        private static string CityToCode(string city)
        {
            string pattern = string.Format("<city id=\"(?<code>.*?)\" name=\"{0}\">", city);
            return Regex.Match(ProvinceAndCity, pattern).Groups["code"].Value;
        }

        #endregion
    }
}
