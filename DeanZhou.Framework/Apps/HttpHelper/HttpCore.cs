using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;

namespace DeanZhou.Framework
{
    /// <summary>
    /// http处理基类
    /// 抽象类
    /// 封装http访问参数对象和帮助对象
    /// 通过继承，实现方便、可扩展和可维护的具体访问类
    /// </summary>
    public class HttpCore
    {
        /// <summary>
        /// 访问实体
        /// </summary>
        public HttpItem CurrentHttpItem { get; set; }

        /// <summary>
        /// 访问执行
        /// </summary>
        protected HttpHelper CurrentHttpHelper { get; set; }

        public HttpCore()
        {
            CurrentHttpItem = new HttpItem();
            CurrentHttpHelper = new HttpHelper();
        }

        /// <summary>
        /// 设置访问url
        /// </summary>
        /// <param name="urlTem"></param>
        /// <param name="ps"></param>
        public virtual void SetUrl(string urlTem, params object[] ps)
        {
            CurrentHttpItem.URL = string.Format(urlTem, ps.Select(o => HttpUtility.UrlEncode(o.ToString())).Cast<object>().ToArray());
        }

        /// <summary>
        /// 根据设置抓取网页源码
        /// </summary>
        /// <returns></returns>
        public virtual string GetHtml()
        {
            var res = CurrentHttpHelper.GetHtml(CurrentHttpItem).Html;
            return res;
        }

        public virtual HtmlNodeCollection SelectNodes(string html, string xpath)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.SelectNodes(xpath);
        }

        public virtual string SelectNodesHtml(string html, string xpath)
        {
            HtmlNodeCollection res = SelectNodes(html, xpath);
            if (res == null)
            {
                return "null";
            }
            return string.Join("\r\n", res.Nodes().Select(c => c.InnerHtml));
        }

        public virtual HtmlNode SelectSingleNode(HtmlNode node, string xpath)
        {
            if (node == null)
            {
                return null;
            }
            xpath = node.XPath + xpath;
            return node.SelectSingleNode(xpath);
        }

        /// <summary>
        /// 匹配单个正则
        /// </summary>
        /// <param name="html">待匹配html</param>
        /// <param name="reg">正则</param>
        /// <returns>匹配结果</returns>
        protected virtual MatchCollection Match(string html, string reg)
        {
            var regex = new Regex(reg, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
            //异步的正则匹配（2000是超时时间）
            return new AsynchronousRegex(2000).Matchs(regex, html);
        }

        /// <summary>
        /// 匹配返回实体（json-> entity）
        /// </summary>
        /// <typeparam name="T">待返回实体</typeparam>
        /// <param name="jsonResult"></param>
        /// <param name="hasError"></param>
        /// <returns>T</returns>
        protected virtual T Match<T>(string jsonResult, out bool hasError)
        {
            try
            {
                hasError = false;
                jsonResult = jsonResult.Trim('[').Trim("]\n".ToArray());
                return jsonResult.ConvertToObject<T>();
            }
            catch (Exception)
            {
                hasError = true;
                return default(T);
            }
        }

        /// <summary>
        /// 匹配返回实体列表（json-> entity list）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonResult"></param>
        /// <returns></returns>
        protected virtual List<T> Match<T>(string jsonResult)
        {
            try
            {
                return jsonResult.ConvertToObject<List<T>>();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }
    }
}
