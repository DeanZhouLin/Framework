using System;
using CommonEntityLib.Entities.user;

namespace SinaWeiboCore.CN
{
    /// <summary>
    /// 获取页面HTML
    /// </summary>
    public static class GetCnPage
    {
        /// <summary>
        /// 根据微博Url获取微博内容页面
        /// </summary>
        /// <param name="web"></param>
        /// <param name="url">m.weibo.cn 微博页面</param>
        /// <returns></returns>
        public static string GetSingleMblog(WebAccessBase web, string url)
        {
            web.Reffer = null;

            return web.GetHTML(url);
        }

        /// <summary>
        /// 根据用户信息和页码获取用户的10条微博
        /// </summary>
        /// <param name="web"></param>
        /// <param name="weiboUser"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public static string GetMultiMblog(WebAccessBase web, Entity weiboUser, int page)
        {
            var reffer = string.Format("http://m.weibo.cn/u/{0}", weiboUser.ID);
            web.Reffer = new Uri(reffer);
            var url = string.Format("http://m.weibo.cn/page/json?containerid={0}_-_WEIBO_SECOND_PROFILE_WEIBO&page={1}", weiboUser.IDStr, page);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取用户主页的信息
        /// </summary>
        /// <param name="web"></param>
        /// <param name="uid">用户</param>
        /// <returns></returns>
        public static string GetUser(WebAccessBase web, string uid)
        {
            web.Reffer = null;
            //获取主页
            var url = string.Format("http://m.weibo.cn/u/{0}", uid);
            return web.GetHTML(url);
        }

        /// <summary>
        /// 获取新增粉丝
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static string GetNewFans(WebAccessBase web)
        {
            web.Reffer = new Uri("http://m.weibo.cn/");
            const string url = "http://m.weibo.cn/p/index?containerid=103103";
            return web.GetHTML(url);
        }
    }
}
