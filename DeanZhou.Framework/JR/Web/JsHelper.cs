using System;
using System.Web;

namespace JFx.Web
{
    /// <summary>
    /// 客户端脚本输出
    /// </summary>
    [Obsolete]
    public class JsHelper
    {
        /// <summary>
        /// 弹出信息,并跳转指定页面。
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="toURL">跳转地址</param>
        public static void AlertAndRedirect(string message, string toURL)
        {
            string js = "<script language=javascript>alert('{0}');window.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, message, toURL));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 弹出信息,并返回历史页面
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="value">history.go(value)</param>
        public static void AlertAndGoHistory(string message, int value)
        {
            string js = @"<Script language='JavaScript'>alert('{0}');history.go({1});</Script>";
            HttpContext.Current.Response.Write(string.Format(js, message, value));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 直接跳转到指定的页面
        /// </summary>
        /// <param name="toURL">跳转地址</param>
        public static void Redirect(string toUrl)
        {
            string js = @"<script language=javascript>window.location.replace('{0}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, toUrl));
        }

        /// <summary>
        /// 弹出信息 并指定到父窗口
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="toURL">跳转地址</param>
        public static void AlertAndParentUrl(string message, string toURL)
        {
            string js = "<script language=javascript>alert('{0}');window.top.location.replace('{1}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, message, toURL));
        }

        /// <summary>
        /// 返回到父窗口
        /// </summary>
        /// <param name="toURL">跳转地址</param>
        public static void ParentRedirect(string ToUrl)
        {
            string js = "<script language=javascript>window.top.location.replace('{0}')</script>";
            HttpContext.Current.Response.Write(string.Format(js, ToUrl));
        }

        /// <summary>
        /// 返回历史页面
        /// </summary>
        /// <param name="value">history.go(value)</param>
        public static void BackHistory(int value)
        {
            string js = @"<Script language='JavaScript'>history.go({0});</Script>";
            HttpContext.Current.Response.Write(string.Format(js, value));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 弹出信息
        /// </summary>
        /// <param name="message">提示信息</param>
        public static void Alert(string message)
        {
            string js = "<script language=javascript>alert('{0}');</script>";
            HttpContext.Current.Response.Write(string.Format(js, message));
        }

        /// <summary>
        /// 注册脚本块
        /// </summary>
        /// <param name="page">page</param>
        /// <param name="scriptString">js脚本</param>
        public static void RegisterScriptBlock(System.Web.UI.Page page, string scriptString)
        {
            page.ClientScript.RegisterStartupScript(page.GetType(), "scriptblock", "<script type='text/javascript'>" + scriptString + "</script>");
        }
    }
}