using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using CsharpHttpHelper.Enum;
using DeanZhou.Framework;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceStack;
using ServiceStack.Common;
using ServiceStack.Text;
using DynamicJson = DeanZhou.Framework.DynamicJson;
using HttpHelper = CsharpHttpHelper.HttpHelper;
using HttpItem = CsharpHttpHelper.HttpItem;
using PostDataType = CsharpHttpHelper.Enum.PostDataType;
using ResultType = DeanZhou.Framework.ResultType;

namespace UnitTestCore
{
    [TestClass]
    public class TestLocalCache
    {
        public static void ExecCrawler(int p)
        {
            CaoQunCrawlerCore cc = new CaoQunCrawlerCore(p, 16);
            cc.ExecCrawler();
        }

        public async static void TData()
        {
            var multiplyBlock = new TransformBlock<int, int>(item =>
            {
                var res = item * 2;
                Console.WriteLine("{0} * 2 = {1}", item, res);
                return res;
            });

            var divideBlock = new TransformBlock<int, int>(item =>
            {
                var res = item / 2;
                Console.WriteLine("{0} / 2 = {1}", item, res);
                return res;
            });

            multiplyBlock.LinkTo(divideBlock);

            multiplyBlock.Post(2);

            multiplyBlock.Complete();
            await divideBlock.Completion;
        }

        [TestMethod]
        public void TestData()
        {
            TData();
        }

        [TestMethod]
        public void TestLogin()
        {
            HttpHelper hp = new HttpHelper();

            HttpItem httpItem = new HttpItem
            {
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.90 Safari/537.36",
                URL = "http://m.weibo.cn",//URL     必需项    
                Method = "GET",//URL     可选项 默认为Get   
                Header = new WebHeaderCollection(),
                CookieCollection = new CookieCollection(),
                KeepAlive = true,
                AutoRedirectCookie = true,
                Allowautoredirect = true//是否根据301跳转     可选项   
            };
            httpItem.Header.Add("Accept-Encoding", "gzip, deflate, sdch");
            httpItem.Header.Add("Accept-Language", "zh-CN,zh;q=0.8");

            var res = hp.GetHtml(httpItem);

            //编码用户名 @替换为%40 进行Base64编码
            string su = "funny_zhoulin@163.com".Replace("@", "%40");
            Byte[] bufin = Encoding.UTF8.GetBytes(su);
            su = Convert.ToBase64String(bufin, 0, bufin.Length);

            var callbackStr = string.Format("jsonpcallback{0}", CommonExtension.GetTime());
            var perLoginUrl = string.Format("https://login.sina.com.cn/sso/prelogin.php?checkpin=1&entry=mweibo&su={0}&callback={1}", su, callbackStr);
            httpItem.URL = perLoginUrl;

            var perLoginHtml = hp.GetHtml(httpItem);

            httpItem.Cookie += perLoginHtml.Cookie;


            string postData = string.Format(
    "username={0}&password={1}&savestate=1{2}&ec=0&pagerefer=https%3A%2F%2Fpassport.weibo.cn%2Fsignin%2Fwelcome%3Fentry%3Dmweibo%26r%3Dhttp%253A%252F%252Fm.weibo.cn%252F%26&entry=mweibo&loginfrom=&client_id=&code=&hff=&hfp=",
    "funny_zhoulin@163.com".Replace("@", "%40"), "zhoulin", "");

            httpItem.Referer = "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=http%3A%2F%2Fm.weibo.cn%2F";
            httpItem.URL = "https://passport.weibo.cn/sso/login";
            httpItem.Postdata = postData;
            httpItem.Method = "post";
            httpItem.ContentType = "application/x-www-form-urlencoded";
            var postHtml = hp.GetHtml(httpItem);

            dynamic postResult = DynamicJson.Parse(postHtml.Html);
            string retcode = postResult.retcode;
            string Uid = postResult.data.uid;

            if (postResult.data.IsDefined("loginresulturl") && !string.IsNullOrEmpty(postResult.data["loginresulturl"]))
            {
                string loginresulturl = postResult.data["loginresulturl"] + "&savestate=1&url=http%3A%2F%2Fm.weibo.cn%2F";
                httpItem.Referer = "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=http%3A%2F%2Fm.weibo.cn%2F";
                httpItem.URL = loginresulturl;
                httpItem.Method = "Get";
                httpItem.ContentType = "";

                var temp0 = hp.GetHtml(httpItem);

            }
            else
            {
                string weibo_com = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["weibo.com"], GetTime());
                httpItem.Referer = "https://passport.weibo.cn/signin/login?entry=mweibo&res=wel&wm=3349&r=http%3A%2F%2Fm.weibo.cn%2F";
                httpItem.URL = weibo_com;
                httpItem.Method = "Get";
                httpItem.ContentType = "";
                var temp1 = hp.GetHtml(httpItem);

                string sina_com_cn = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["sina.com.cn"], GetTime());
                httpItem.URL = weibo_com;
                httpItem.Method = "Get";
                httpItem.ContentType = "";
                var temp2 = hp.GetHtml(httpItem);

                string weibo_cn = string.Format("https:{0}&savestate=1&callback=jsonpcallback{1}", postResult.data.crossdomainlist["weibo.cn"], GetTime());
                httpItem.URL = weibo_cn;
                httpItem.Method = "Get";
                httpItem.ContentType = "";
                var temp3 = hp.GetHtml(httpItem);
            }
        }

        public static double GetTime()
        {
            DateTime minValue = new DateTime(1970, 1, 1);
            DateTime nowValue = DateTime.Now;
            double value = (nowValue - minValue).TotalMilliseconds;
            return Math.Floor(value);
        }


        public static async Task TT()
        {
            await Task.Delay(1000 * 10);
        }

        [TestMethod]
        public void Test1()
        {
            Task t = TT();
            Task.WaitAll(t);
            Observable.Interval(TimeSpan.FromSeconds(1)).Timestamp().Where(c => c.Value % 10 == 0).Select(c => c.Value)
                .Subscribe(x =>
                {
                    LogHelper.CustomInfo(x, "Observable");
                });
            while (true)
            {
                if (false)
                {
                    break;
                }
                Thread.Sleep(5000);
            }
        }

        [TestMethod]
        public void Test()
        {
            Parallel.For(0, 100, i =>
            {
                DateTime dt = SysTimeRecord.Get(i.ToString());
                SysTimeRecord.Set(i.ToString(), dt == DateTime.MinValue ? DateTime.Now.AddDays(-i) : dt.AddHours(1));
            });

            for (int i = 0; i < 100; i++)
            {
                DateTime dt = SysTimeRecord.Get(i.ToString());
                SysTimeRecord.Set(i.ToString(), dt == DateTime.MinValue ? DateTime.Now.AddDays(i) : dt.AddHours(-1));
            }
            RedisTest.Test();


            AsyncWorker<int> aw = new AsyncWorker<int>(3, Console.WriteLine);
            for (int i = 0; i < 100; i++)
            {
                aw.AddItem(i);
            }
            aw.StopAndWait();


            Thread.Sleep(10000000);

            Parallel.Invoke(
                () => ExecCrawler(1),
                () => ExecCrawler(2),
                () => ExecCrawler(3),
                () => ExecCrawler(4),
                () => ExecCrawler(5)
                );



            HttpCore hc = new HttpCore();
            hc.SetUrl("http://ac168.info/bt/thread.php?fid=16&page={0}", 2);
            var res = hc.GetHtml();
            var tres = res.SelectNodes("//*[@id='ajaxtable']/tbody[1]/tr[@class='tr3 t_one']");

            HtmlNode node = tres[10];
            var an = node.SelectSingleNode("/td[2]/h3/a");

            string aHref = "http://ac168.info/bt/" + an.Attributes["href"].Value;
            string aText = an.InnerHtml;


            hc.SetUrl(aHref);
            res = hc.GetHtml();
            tres = res.SelectNodes("//*[@id='read_tpc']/img");

            string imgUrl = tres[0].Attributes["src"].Value;

            HttpCore hcImg = new HttpCore();
            hcImg.SetUrl(imgUrl);
            hcImg.CurrentHttpItem.ResultType = ResultType.Byte;
            //得到HTML代码
            HttpResult result = hcImg.CurrentHttpHelper.GetHtml(hcImg.CurrentHttpItem);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //表示访问成功，具体的大家就参考HttpStatusCode类
            }
            //表示StatusCode的文字说明与描述
            string statusCodeDescription = result.StatusDescription;
            //把得到的Byte转成图片
            Image img = result.ResultByte.ByteArrayToImage();
            img.Save("f://imgs/" + imgUrl.Split('/').Last());
            Console.WriteLine(tres);
            Console.ReadLine();
            RequestBase rb = new RequestBase();
            rb.BindLocalCache("intc", s => GetSourceCacheInt(s[0].TryChangeType(0)), 3);
            rb.BindLocalCache("person", s => GetSourceCachePerson(), null);

            Stopwatch st0 = new Stopwatch();
            st0.Start();
            rb.BindLocalCache("ps", s => GetSourceCacheListPerson(), null);
            st0.Stop();
            Console.WriteLine("实例化1千万个对象，耗时【{0}】毫秒", st0.ElapsedMilliseconds);

            Stopwatch st1 = new Stopwatch();
            st1.Start();
            for (int i = 0; i < 10000000; i++)
            {
                rb.BindLocalCache("ps", s => GetSourceCacheListPerson(), null);
            }
            st1.Stop();
            Console.WriteLine("绑定1千万次本地缓存，耗时【{0}】毫秒", st1.ElapsedMilliseconds);

            var t1 = rb.GetLocalCache("intc");
            var t2 = rb.GetLocalCache("person");
            var t3 = rb.GetLocalCache<Person>("person");
            var t4 = rb.LocalCache.person;
            var t5 = rb.LocalCache.intc;

            Stopwatch st = new Stopwatch();
            st.Start();
            for (int i = 0; i < 10000000; i++)
            {
                var t = rb.LocalCache.ps;
                var tt = rb.GetLocalCache<List<Person>>("ps");
            }
            st.Stop();
            Console.WriteLine("读取2千万次本地缓存，耗时【{0}】毫秒", st.ElapsedMilliseconds);
        }

        private static int GetSourceCacheInt(int s)
        {
            return s;
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }

        }

        private static Person GetSourceCachePerson()
        {
            return new Person
            {
                Age = 1,
                Name = "zl"
            };
        }

        private static List<Person> GetSourceCacheListPerson()
        {
            List<Person> ps = new List<Person>();
            for (int i = 0; i < 10000000; i++)
            {
                ps.Add(new Person
                {
                    Age = i,
                    Name = i.ToString()
                });
            }
            return ps;
        }
    }
}
