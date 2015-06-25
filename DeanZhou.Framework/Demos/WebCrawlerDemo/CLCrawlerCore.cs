using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace DeanZhou.Framework
{
    public class CLCrawlerCore
    {

        private const string CLUrlTemplate = "http://5.yao.cl/thread0806.php?fid=2&search=&page={0}";

        public string CaoQunUrl
        {
            get;
            set;
        }

        public CLCrawlerCore(int pageIndex)
        {
            CaoQunUrl = string.Format(CLUrlTemplate, pageIndex);
        }


        private IEnumerable<MainItem> GetCaoQunMainItems()
        {
            List<MainItem> res = new List<MainItem>();
            HttpCore hc = new HttpCore();
            hc.SetUrl(CaoQunUrl);
            hc.CurrentHttpItem.Allowautoredirect = true;
            string mainHtml = hc.GetHtml();

            HtmlNodeCollection mainItems = mainHtml.SelectNodes("//*[@id='ajaxtable']/tbody[1]/tr");
            if (mainItems == null)
            {
                return new List<MainItem>();
            }
            foreach (HtmlNode mainItem in mainItems)
            {
                var an = mainItem.SelectSingleNode(mainItem.XPath + "/td[2]/h3/a");
                if (an == null)
                {
                    continue;
                }
                string infoUrl = "http://5.yao.cl/" + an.Attributes["href"].Value;
                string title = an.InnerText;

                MainItem temp = new MainItem
                {
                    HPLDetailItem = new HPLDetailItem(),
                    InfoUrl = infoUrl,
                    Title = title,
                    Url = CaoQunUrl
                };
                res.Add(temp);
            }
            return res;
        }

        public void ExecCrawler()
        {
            IEnumerable<MainItem> mainItems = GetCaoQunMainItems();

            Parallel.ForEach(mainItems, StoreDB);
        }

        private static void StoreDB(MainItem caoQunMainItem)
        {
            DapperHelper dh = DapperHelper.GetInstance("Data Source=.;Initial Catalog=DeanDB;Integrated Security=True");
            const string sqlFormat =
@"INSERT INTO [dbo].[WebCrawlerResult]
           ([Title]
           ,[PicDic]
           ,[Url]
           ,[Remark])
     VALUES
           ('{0}'
           ,'{1}'
           ,'{2}'
           ,'{3}')";
            string sql = string.Format(sqlFormat,
                caoQunMainItem.Title,
                string.Empty,
                caoQunMainItem.InfoUrl,
                caoQunMainItem.Remark);
            dh.Execute(sql);
        }

    }

}
