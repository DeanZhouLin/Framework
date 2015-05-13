using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CsharpHttpHelper;

namespace CsharpHttpHelper_Demo
{
    public partial class Text : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //1.解决301跳转时Cookie不累加的问题，当是站内网址是无法解析的问题，比如 /a.aspx就没有办法访问了 已解决
            //2.解决Cookie精简化方法GetSmallCookie()的判断重复时的Bug  已解决
            //3.修复   if (ResponseByte != null & ResponseByte.Length > 0)会报错的Bug  已解决
            //4.RedirectUrl解决空异常的Bug 已解决
            //5.修复StrCookieToCookieCollection()方法当Cookie名字有空格时的Bug

        }
    }
}