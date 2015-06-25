using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;

namespace DeanZhou.Framework
{
    public class HPLCrawlerCore
    {

        private const string CaoQunUrlTemplate = "http://www.haipilu.net/vodlist/6_{0}.html";

        public string CaoQunUrl
        {
            get;
            set;
        }

        public HPLCrawlerCore(int pageIndex)
        {
            CaoQunUrl = string.Format(CaoQunUrlTemplate, pageIndex);
        }

        private string kM = null;

        private void ThreadWebBrowser(string url)
        {
            Thread tread = new Thread(BeginCatch);
            tread.SetApartmentState(ApartmentState.STA);
            tread.Start(url);
        }

        private void BeginCatch(object obj)
        {
            string url = obj.ToString();
            WebBrowser wb = new WebBrowser();
            wb.ScriptErrorsSuppressed = true;
            //在这里Navigate一个空白页面
            wb.Navigate("about:blank");
            string htmlcode = GetHtmlSource(url);
            wb.Document.Write(htmlcode);
            
            kM = wb.Document.Body.InnerHtml;
        }

        private string GetHtmlSource(string Url)
        {
            string text1 = "";
            try
            {
                System.Net.WebClient wc = new WebClient();
                text1 = wc.DownloadString(Url);
            }
            catch (Exception exception1)
            { }
            return text1;
        }

        private List<MainItem> GetCaoQunMainItems()
        {
            List<MainItem> res = new List<MainItem>();
            HttpCore hc = new HttpCore();
            hc.SetUrl(CaoQunUrl);
            string mainHtml = hc.GetHtml();

            //MSScriptControl.ScriptControlClass scc = new MSScriptControl.ScriptControlClass { Language = "javascript" };
            //mainHtml = scc.Eval(mainHtml).ToString();

            ThreadWebBrowser(CaoQunUrl);
            while (kM == null)
            {
                Thread.Sleep(1000);
            }
            HtmlNodeCollection mainItems = mainHtml.SelectNodes("/html/body/div[8]/div[2]/div[@class='list-pianyuan-box']");
            if (mainItems == null)
            {
                return new List<MainItem>();
            }
            foreach (HtmlNode mainItem in mainItems)
            {
                var an = mainItem.SelectSingleNode("/div[1]/div[2]/div[1]/a");

                string infoUrl = "http://www.haipilu.net/" + an.Attributes["href"].Value;
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
            List<MainItem> mainItems = GetCaoQunMainItems();

            Parallel.ForEach(mainItems, caoQunMainItem =>
            {
                HttpCore hc1 = new HttpCore();
                hc1.SetUrl(caoQunMainItem.InfoUrl);
                string infoHtml = hc1.GetHtml();
                HtmlNodeCollection imgNodes = infoHtml.SelectNodes("/html/body/div[7]/div/div[4]/div[2]/div/img");
                if (imgNodes == null)
                {
                    return;
                }

                string remark = infoHtml.SelectNodes("/html/body/div[7]/div/div[4]/div[2]/div/img")[0].InnerText;
                caoQunMainItem.Remark = remark;
                Parallel.ForEach(imgNodes, htmlNode =>
                {
                    string imgUrl = htmlNode.Attributes["src"].Value;
                    HttpCore hc = new HttpCore { CurrentHttpItem = { ResultType = ResultType.Byte } };
                    hc.SetUrl(imgUrl);
                    HttpResult temp = hc.GetHttpResult();
                    //把得到的Byte转成图片
                    Image img = temp.ResultByte.ByteArrayToImage();
                    if (img == null)
                    {
                        return;
                    }
                    SaveImg("img5", caoQunMainItem, img, imgUrl);
                });

                //数据入库
                StoreDB(caoQunMainItem);
            });

        }

        private static void SaveImg(string imgFileName, MainItem caoQunMainItem, Image img, string imgUrl)
        {
            try
            {
                caoQunMainItem.HPLDetailItem.PicUrl = imgUrl;
                Console.WriteLine(imgUrl);

                if (!Directory.Exists("F://" + imgFileName + "/" + caoQunMainItem.Title + "/"))
                {
                    Directory.CreateDirectory("F://" + imgFileName + "/" + caoQunMainItem.Title + "/");
                }
                string pic = caoQunMainItem.HPLDetailItem.PicDic = "F://" + imgFileName + "/" + caoQunMainItem.Title + "/" + imgUrl.Split('/').Last();

                img.Save(pic.Trim('?'));
                img.Dispose();
            }
            catch (Exception)
            {

            }
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
            string sql = string.Format(sqlFormat, caoQunMainItem.Title, caoQunMainItem.HPLDetailItem.PicDic,
                caoQunMainItem.InfoUrl, caoQunMainItem.Remark);
            dh.Execute(sql);
        }

    }

    public class MainItem
    {
        public string InfoUrl { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string Remark { get; set; }

        public HPLDetailItem HPLDetailItem { get; set; }
    }

    public class HPLDetailItem
    {
        public string PicDic { get; set; }

        public string PicUrl { get; set; }
    }
}
