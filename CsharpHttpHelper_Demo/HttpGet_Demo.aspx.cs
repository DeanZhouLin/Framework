using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CsharpHttpHelper;
using System.Net;
using System.Text;
using CsharpHttpHelper.Enum;

namespace CsharpHttpHelper_Demo
{
    public partial class HttpGet_Demo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


            HttpHelper http = new HttpHelper();
            HttpItem item = new HttpItem()
            {
                URL = "http://workroom.sufeinet.com/Hezuo.aspx",//URL     必需项
            };
            HttpResult result = http.GetHtml(item);
            string html = result.Html;
            string cookie = result.Cookie;
            string urlll = result.RedirectUrl;

            Response.Write(result.Html);
            ////===================================状态码======================================


            ////创建Httphelper对象
            //HttpHelper http = new HttpHelper();
            ////创建Httphelper参数对象
            //HttpItem item = new HttpItem()
            //{
            //    URL = "http://www.sufeinet.com",//URL     必需项    
            //    Method = "get",//URL     可选项 默认为Get   
            //    ContentType = "text/html",//返回类型    可选项有默认值   
            //    //ContentType = "application/x-www-form-urlencoded",//返回类型    可选项有默认值   
            //};
            ////请求的返回值对象
            //HttpResult result = http.GetHtml(item);
            ////获取请请求的Html
            //string html = result.Html;
            ////获取请求的Cookie
            //string cookie = result.Cookie;

            ////状态码
            //HttpStatusCode code = result.StatusCode;
            ////状态描述
            //string Des = result.StatusDescription;
            //if (code == HttpStatusCode.OK)
            //{
            //    //状态为200
            //}
        }
    }
}