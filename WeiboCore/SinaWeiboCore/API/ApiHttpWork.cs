using System;
using System.Collections.Generic;
using CommonEntityLib.Entities.comment;
using CommonEntityLib.Entities.message;
using CommonEntityLib.Entities.tag;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Collection = CommonEntityLib.Entities.user.Collection;

namespace SinaWeiboCore.API
{
    /// <summary>
    /// 
    /// </summary>
    public class ApiHttpWork : IHttpWork
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly ApiHttpWork Instance = new ApiHttpWork();

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
            throw new NotImplementedException();
        }

        public List<string> AddPicture(IWeiboLogin webLogin, params byte[][] pics)
        {
            throw new NotImplementedException();
        }

        public List<string> AddPicture(IWeiboLogin webLogin, params string[] picPaths)
        {
            throw new NotImplementedException();
        }

        public string FriendCreate(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public string FriendDestroy(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public string AttitudesCreate(IWeiboLogin webLogin, string weiboUrl, string weiboId)
        {
            throw new NotImplementedException();
        }

        public string DelMblog(IWeiboLogin webLogin, string url, string mid)
        {
            throw new NotImplementedException();
        }

        public string UploadAvatar(IWeiboLogin webLogin, string cloneUid)
        {
            throw new NotImplementedException();
        }

        public string AttitudesDestroy(IWeiboLogin webLogin, string weiboUrl, string weiboId)
        {
            throw new NotImplementedException();
        }

        public string SaveInformation(IWeiboLogin webLogin, string targetUid, string uid, string nickname)
        {
            throw new NotImplementedException();
        }

        public string SendDirectMessages(IWeiboLogin webLogin, string uid, string text)
        {
            throw new NotImplementedException();
        }

        public List<CnDirectMessagesEntity> RecevieDirectMessages(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public List<CnDirectMessagesEntity> RecevieAllDirectMessages(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public string SendReplyComment(IWeiboLogin webLogin, string uid, string mid, string cid, string replyName, string replyContent)
        {
            throw new NotImplementedException();
        }

        public List<Entity> ReceiveComment(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public CommonEntityLib.Entities.user.Entity GetUserEntity(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public CommonEntityLib.Entities.user.Entity GetUserEntity(string uid)
        {
            throw new NotImplementedException();
        }

        public Collection GetUserEntityColloction(string appKey, string uids)
        {
            string url = string.Format("http://api.weibo.com/2/users/show_batch.json?source={0}&uids={1}", appKey, uids);
            string html = new WebAccessBase().GetHTML(url);
            if (html == null)
            {
                return null;
            }
            var t = JsonConvert.DeserializeObject<Collection>(html);
            return t;
        }

        public Dictionary<string, IEnumerable<Tag>> GetTags(string appKey, string uids)
        {
            string url = string.Format("http://api.weibo.com/2/tags/tags_batch.json?source={0}&uids={1}", appKey, uids);
            string html = new WebAccessBase().GetHTML(url);
            if (html == null)
            {
                return null;
            }

            var json = JArray.Parse(html);
            var result = new Dictionary<string, IEnumerable<Tag>>();
            foreach (var item in json)
            {
                var entry = item["id"].ToString();
                List<Tag> list = new List<Tag>();
                foreach (var jToken in item["tags"])
                {
                    var obj = (JObject)jToken;
                    var first = (JProperty)obj.First;
                    var last = (JProperty)obj.Last;

                    list.Add(new Tag { ID = first.Name, Name = string.Format("{0}", first.Value), Weight = string.Format("{0}", last.Value) });

                }
                result.Add(entry, list);
            }
            return result;
        }

        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(string uid)
        {
            throw new NotImplementedException();
        }

        public List<CommonEntityLib.Entities.status.Entity> GetMblogs(WebAccessBase web, string uid, string mid, int page)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(IWeiboLogin webLogin, string uid)
        {
            throw new NotImplementedException();
        }

        public bool UserExist(string uid)
        {
            throw new NotImplementedException();
        }

        public List<string> GetNewFans(IWeiboLogin webLogin)
        {
            throw new NotImplementedException();
        }

        public CommonEntityLib.Entities.status.Entity GetMblog(IWeiboLogin webLogin, string url)
        {
            throw new NotImplementedException();
        }

        public List<long> GetLastAttentionUIDs(string uid)
        {
            throw new NotImplementedException();
        }

        public List<long> GetPrewAttentionUIDs(string uid)
        {
            throw new NotImplementedException();
        }

        public List<long> Get20Fans(string uid)
        {
            throw new NotImplementedException();
        }

        public List<long> GetAttentionUIDsAsParallel(string uid)
        {
            throw new NotImplementedException();
        }

        public List<string> GetWeiboHttpSearchUserIDs(int factor1, int factor2, int factor3, int factor4, string queryWord)
        {
            throw new NotImplementedException();
        }
    }
}
