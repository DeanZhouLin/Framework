using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommonEntityLib.Entities.cnpage;
using CommonEntityLib.Entities.status;
using Newtonsoft.Json;
using NLog;
using System.IO;
using SinaWeiboCore.ReffDll;

namespace SinaWeiboCore.CN
{
    /// <summary>
    /// 分析CN页面
    /// </summary>
    public static class AnalyseCnPage
    {
        private static readonly Logger AnalyseCnPageLogger = LogManager.GetLogger("AnalyseCnPage");

        /// <summary>
        /// 分析单页微博
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static Entity AnalysisSingleWeiboPage(string html)
        {
            var jsonStr = GetSingleWeiboPageJsonStr(html);
            return JsonConvert.DeserializeObject<Entity>(jsonStr);
        }

        /// <summary>
        /// 分析多条微博页面
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<Entity> AnalysisMultiWeiboPage(string html)
        {
            try
            {
                if (html.Contains(@"\u6ca1\u6709\u5185\u5bb9"))
                    return new List<Entity>();

                var weiboPage = JsonConvert.DeserializeObject<CnPage>(html);
                var card = weiboPage.Cards.FirstOrDefault();
                if (card != null)
                {
                    return card.CardGroups.Select(p => p.Mblog).ToList();
                }
            }
            catch (Exception exception)
            {
                AnalyseCnPageLogger.Error(string.Format("分析博文页面失败\r\n{0}", html), exception);
            }
            return new List<Entity>();
        }

        private static string GetSingleWeiboPageJsonStr(string html)
        {
            var index = html.IndexOf(@"""card_type"":9,""mblog"":{", StringComparison.Ordinal);
            html = html.Substring(index);
            html = html.Replace(@"""card_type"":9,""mblog"":", "");
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
            html = html.Substring(0, last + 1);
            return html;
        }

        /// <summary>
        /// 分析用户主页信息
        /// </summary>
        /// <param name="home"></param>
        public static CommonEntityLib.Entities.user.Entity AnalysisUserHome(string home)
        {
            try
            {
                CommonEntityLib.Entities.user.Entity weiboUser = new CommonEntityLib.Entities.user.Entity
                {
                    ID = Regex.Match(home, "\"id\":\"(\\d*?)\"").Groups[1].Value
                };
                //分析uid
                if (string.IsNullOrEmpty(weiboUser.ID))
                    return null;

                //获取分页爬博文所要信息
                weiboUser.IDStr = Regex.Match(home, "'stageId':'(\\d*?)'").Groups[1].Value;

                //获取用户发表的微博数量
                var mblogNumStr = Regex.Match(home, "\"mblogNum\":\"(\\d*?)\"").Groups[1].Value;
                weiboUser.StatusesCount = int.Parse(mblogNumStr);

                //获取关注数量
                var attNumStr = Regex.Match(home, "\"attNum\":\"(\\d*?)\"").Groups[1].Value;
                weiboUser.FriendsCount = int.Parse(attNumStr);

                //获取粉丝数量
                var fansNumStr = Regex.Match(home, "\"fansNum\":\"(\\d*?)\"").Groups[1].Value;
                weiboUser.FollowersCount = int.Parse(fansNumStr);

                //获取用户性别
                var genderStr = Regex.Match(home, "\"ta\":\"(.*?)\"").Groups[1].Value;
                if (genderStr == @"\u4ed6")
                    weiboUser.Gender = "m";

                //用户昵称
                var nickname = Regex.Match(home, "\"name\":\"(.*?)\"").Groups[1].Value;
                weiboUser.ScreenName = nickname.NormalU2C();

                //用户所在省份
                var location = Regex.Match(home, "\"nativePlace\":\"(.*?)\"").Groups[1].Value;
                weiboUser.Location = location.NormalU2C();

                //用户简介
                var description = Regex.Match(home, "\"description\":\"(.*?)\"").Groups[1].Value;
                weiboUser.Description = description.NormalU2C();

                //头像地址
                var profileImageUrl = Regex.Match(home, @"""profile_image_url"":""(?<url>.*?)""").Groups["url"].Value;
                weiboUser.ProfileImageUrl = profileImageUrl.Replace(@"\", "");

                return weiboUser;
            }
            catch (Exception exception)
            {
                AnalyseCnPageLogger.Error(string.Format("分析用户主页信息失败\r\n{0}", home), exception);
                return null;
            }
        }

        /// <summary>
        /// 分析多条微博页面
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<Entity> AnalysisWeiboPage(string html)
        {
            try
            {
                var weiboPage = JsonConvert.DeserializeObject<CnPage>(html);
                var card = weiboPage.Cards.FirstOrDefault();
                if (card != null)
                {
                    return card.CardGroups.Select(p => p.Mblog).ToList();
                }
            }
            catch (Exception exception)
            {
                AnalyseCnPageLogger.Error(string.Format("分析博文页面失败\r\n{0}", html), exception);
            }
            return null;
        }

        public static IEnumerable<long> AnalysisFollowers(string html)
        {
            if (string.IsNullOrEmpty(html))
                return null;
            //网站返回的错误
            if (html.Contains("对不起，您浏览的网页出错(异常)了！"))
                return null;
            //该页面数据为空
            if (html.Contains(@"mod\/empty"))
                return new List<long>();
            try
            {
                var followersPage = JsonConvert.DeserializeObject<CnPage>(html);
                var card = followersPage.Cards.FirstOrDefault();
                if (card != null)
                {
                    return card.CardGroups.Select(p => long.Parse(p.User.ID ?? "-1"));
                }
            }
            catch (Exception exception)
            {
                AnalyseCnPageLogger.Info(html);
                AnalyseCnPageLogger.Error(exception);
            }
            return null;
        }

        public static IEnumerable<long> AnalysisFans(string html)
        {
            return AnalysisFollowers(html);
        }

        /// <summary>
        /// 分析新增粉丝页面
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static List<string> AnalysisNewFansPage(string html)
        {
            try
            {
                if (html.Contains(@"\u6682\u65f6\u6ca1\u6709\u65b0\u7684\u7c89\u4e1d"))
                    return new List<string>();
                html = GetGetNewFansPageJsonStr(html);

                List<string> list = new List<string>();
                dynamic json = DynamicJson.Parse(html);
                foreach (var item in json.card_group[0].card_group)
                {
                    string uid = item.user.id;
                    list.Add(uid);
                }
                return list;
            }
            catch (Exception exception)
            {
                AnalyseCnPageLogger.Error(string.Format("分析新增粉丝页面失败\r\n{0}", html), exception);
            }
            return new List<string>();
        }

        private static string GetGetNewFansPageJsonStr(string html)
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
            html = html.Substring(0, last + 1);
            return html;
        }

        private static void Debug()
        {
            var f001 = File.ReadAllText("001.txt");
            var uidList = AnalysisNewFansPage(f001);
            var f002 = File.ReadAllText("002.txt");
            var uidList2 = AnalysisNewFansPage(f002);
            var f003 = File.ReadAllText("003.txt");
            var uidList3 = AnalysisNewFansPage(f003);
        }
    }
}
