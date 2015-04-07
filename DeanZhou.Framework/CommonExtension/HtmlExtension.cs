using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace DeanZhou.Framework
{
    public static class HtmlExtension
    {
        /// <summary>
        /// 通过xPath抓取Nodes
        /// </summary>
        /// <param name="html"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNodeCollection SelectNodes(this string html, string xpath)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return doc.DocumentNode.SelectNodes(xpath);
        }

        /// <summary>
        /// 通过xPath抓取Nodes的内部html
        /// </summary>
        /// <param name="html"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static string SelectNodesHtml(this string html, string xpath)
        {
            HtmlNodeCollection res = SelectNodes(html, xpath);
            if (res == null)
            {
                return "null";
            }
            return string.Join("\r\n", res.Nodes().Select(c => c.InnerHtml));
        }

        /// <summary>
        /// 通过xPath抓取Node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="xpath"></param>
        /// <returns></returns>
        public static HtmlNode SelectSingleNode(this HtmlNode node, string xpath)
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
        public static MatchCollection Match(this string html, string reg)
        {
            var regex = new Regex(reg, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.CultureInvariant);
            //异步的正则匹配（2000是超时时间）
            return new AsynchronousRegex(2000).Matchs(regex, html);
        }
    }
}
