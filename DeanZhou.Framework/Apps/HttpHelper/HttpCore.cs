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
        public HttpHelper CurrentHttpHelper { get; set; }

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
            var res = GetHttpResult().Html;
            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual HttpResult GetHttpResult()
        {
            var res = CurrentHttpHelper.GetHtml(CurrentHttpItem);
            return res;
        }

    }
}
