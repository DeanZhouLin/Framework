using System;
using System.Collections.Generic;
using System.Diagnostics;
using DeanZhou.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestCore
{
    [TestClass]
    public class TestLocalCache
    {
        [TestMethod]
        public void Test()
        {
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
