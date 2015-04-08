using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using DeanZhou.Framework;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

        [TestMethod]
        public void Test()
        {

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
