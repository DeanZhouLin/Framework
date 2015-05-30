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

        private const string CaoQunUrlTemplate = "http://5.yao.cl/thread0806.php?fid=2&search=&page={0}";

        public string CaoQunUrl
        {
            get;
            set;
        }

        public CLCrawlerCore(int pageIndex)
        {
            CaoQunUrl = string.Format(CaoQunUrlTemplate, pageIndex);
        }


        private IEnumerable<HPLMainItem> GetCaoQunMainItems()
        {
            List<HPLMainItem> res = new List<HPLMainItem>();
            HttpCore hc = new HttpCore();
            hc.SetUrl(CaoQunUrl);
            hc.CurrentHttpItem.Allowautoredirect = true;
            string mainHtml = hc.GetHtml();

            HtmlNodeCollection mainItems = mainHtml.SelectNodes("//*[@id='ajaxtable']/tbody[1]/tr");
            if (mainItems == null)
            {
                return new List<HPLMainItem>();
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

                HPLMainItem temp = new HPLMainItem
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
            IEnumerable<HPLMainItem> mainItems = GetCaoQunMainItems();

            Parallel.ForEach(mainItems, caoQunMainItem =>
            {
                HttpCore hc1 = new HttpCore();
                hc1.SetUrl(caoQunMainItem.InfoUrl);
                string infoHtml = hc1.GetHtml();
                HtmlNodeCollection imgNodes = infoHtml.SelectNodes("//img");
                if (imgNodes == null)
                {
                    return;
                }

                caoQunMainItem.Remark = HttpUtility.UrlEncode(infoHtml);
                //Parallel.ForEach(imgNodes, htmlNode =>
                //{
                //    string imgUrl = htmlNode.Attributes["src"].Value;
                //    HttpCore hc = new HttpCore { CurrentHttpItem = { ResultType = ResultType.Byte } };
                //    hc.SetUrl(imgUrl);
                //    HttpResult temp = hc.GetHttpResult();
                //    //把得到的Byte转成图片
                //    Image img = temp.ResultByte.ByteArrayToImage();
                //    if (img == null)
                //    {
                //        return;
                //    }
                //    SaveImg("img5" + "/" + caoQunMainItem.Title + "/", caoQunMainItem, img, imgUrl);
                //    SaveImg("imgall/", caoQunMainItem, img, imgUrl);
                //});

                //数据入库
                StoreDB(caoQunMainItem);
            });

        }

        private static void SaveImg(string imgFileName, HPLMainItem caoQunMainItem, Image img, string imgUrl)
        {
            try
            {
                caoQunMainItem.HPLDetailItem.PicUrl = imgUrl;
                Console.WriteLine(imgUrl);

                if (!Directory.Exists("F://" + imgFileName))
                {
                    Directory.CreateDirectory("F://" + imgFileName);
                }
                string pic = caoQunMainItem.HPLDetailItem.PicDic = "F://" + imgFileName + imgUrl.Split('/').Last();

                img.Save(pic.Trim('?'));
                img.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private static void StoreDB(HPLMainItem caoQunMainItem)
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
            string sql = string.Format(sqlFormat, caoQunMainItem.Title, caoQunMainItem.HPLDetailItem.PicDic,
                caoQunMainItem.InfoUrl, caoQunMainItem.Remark);
            dh.Execute(sql);
        }

    }

}
