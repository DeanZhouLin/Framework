using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeanZhou.Framework;
using JFx.Utils;

namespace ConsoleTest
{
    class Program
    {
        public static void ExecCrawler(int p, int t)
        {
            CaoQunCrawlerCore cc = new CaoQunCrawlerCore(p, t);
            cc.ExecCrawler();
        }
        static void Main(string[] args)
        {
            for (int i = 0; i < 100000; i++)
            {
                T2 t2 = new T2 { ID = i, T2Name = "fasdfasd" + i };
                t2.Insert();
                LocalDBTest lt = new LocalDBTest { Age = 10, ID = 1, Name = "zhoulin" + i, T2S = new List<T2> { t2 } };
                lt.Insert();
                Thread.Sleep(100);
            }


            //List<LocalDBTest> lts = LocalDB.Select<LocalDBTest>();
            //if (lts == null || !lts.Any())
            //{
            //    T2 t2 = new T2 { ID = 23, T2Name = "fasdfasd" };
            //    t2.Insert();
            //    LocalDBTest lt = new LocalDBTest { Age = 10, ID = 1, Name = "zhoulin", T2S = new List<T2> { t2 } };
            //    lt.Insert();
            //}
            //else
            //{
            //    LocalDBTest lt = lts.First();
            //    if (lt.remark == null)
            //    {
            //        lt.remark = new List<string>();
            //    }
            //    if (lt.T2S == null)
            //    {
            //        lt.T2S = new List<T2> { new T2 { ID = 23, T2Name = "fasdfasd" } };
            //    }
            //    lt.remark.Add(new Random().Next().ToString());
            //    lt.Update();

            //    lt = new LocalDBTest { Age = 10, ID = 1, Name = "zhoulin_" + new Random().Next() };
            //    lt.Insert();
            //}

            Person pson = new Person { id = "1", Name = "zl" };
            Thread.Sleep(10000 * 323);
            string rrr = SerializerHelper.JsonSerializer(pson);
            pson = SerializerHelper.JsonDeserialize<Person>("{\"id\":\"1\",\"Int_id\":3,\"Name\":\"zl\"}");

            DataBufferPool<int> dp = new DataBufferPool<int>(tls =>
            {
                Console.WriteLine(tls.Count + "|" + string.Join(",", tls));
            }, 2000);

            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                dp.AddItem(i % 10);
                Thread.Sleep(r.Next(10, 300));
            }

            Console.ReadLine();
            Parallel.Invoke(
                () => ExecCrawler(30, 16),
                () => ExecCrawler(31, 16),
                () => ExecCrawler(32, 16),
                   () => ExecCrawler(33, 16),
                () => ExecCrawler(30, 4),
                () => ExecCrawler(31, 4),

                () => ExecCrawler(32, 4),
                () => ExecCrawler(33, 4)
                );
            Console.WriteLine("done");
            //DataBufferPool<int> dp = new DataBufferPool<int>(Console.WriteLine);
            //int iu = 0;
            //while (iu < 20)
            //{
            //    iu++;
            //    dp.AddItem(iu);
            //}
            //Console.ReadLine();

            #region 引用类型测试

            RTest rt = new RTest();
            InnerC ic = new InnerC();
            ic.T(rt);
            Console.WriteLine(rt.i);

            ic.T(ref rt);
            Console.WriteLine(rt.i);
            Console.ReadLine();
            #endregion

            #region 动态类型测试

            dynamic dynamicobj = new LocalCacheContainer();
            dynamicobj.Name = "Learning Hard";
            dynamicobj.Age = "24";
            var res = dynamicobj.Age;
            Console.WriteLine("fsadf");

            DynamicObjectTest dot = new DynamicObjectTest();

            dot.LocalCache.Person = new Person
            {
                id = "1",
                Name = "zl"
            };

            dot.LocalCache.OtherInfo = "234";

            Person p1 = dot.LocalCache.Person1 as Person;

            Console.WriteLine(dot.LocalCache.Person.Name);
            Console.WriteLine(dot.LocalCache.OtherInfo);


            Console.ReadLine();
            #endregion

            #region 反射输出对象属性测试
            Group gp = new Group
            {
                GroupID = 1,
                GroupName = "TestGroupName",
                PS = new List<Person>
                {
                    new Person{id = "1",Name = "n1"},
                    new Person{id = "2",Name = "n2"},
                    new Person{id = "3",Name = "n3"}
                },
                Mark = "这是一个测试组"
            };

            foreach (string str in gp.GetReflectPropsValue().Split('|'))
            {
                Console.WriteLine(str);
            }
            Console.ReadLine();
            #endregion

            #region 随机数测试

            TestRandomNum();

            #endregion

            #region 反射对象属性测试
            Person p = new Person();
            TestReflectProps(p);
            Console.ReadLine();
            #endregion

            #region 数据缓存仓库测试

            //key
            const string key = "GetCurrDateKey";
            const string key_Dt = "GetDatatableKey";

            //初始化仓库
            DataWarehouse<string>.InitDataItem(key, GetCurrDate, 1);
            DataWarehouse<DataTable>.InitDataItem(key_Dt, GetDataTable, 1);

            //根据key获取值
            Console.WriteLine(DataWarehouse<string>.GetData(key));
            //Console.WriteLine(DataWarehouse<string>.GetData(key));
            ////休眠 等待过期
            //Thread.Sleep(1000 * 61);
            ////再次根据key获取值
            //Console.WriteLine(DataWarehouse<string>.GetData(key));

            //Console.ReadLine();

            #endregion

            #region DataTable测试

            DataTable dt = DataWarehouse<DataTable>.GetData(key_Dt);

            List<Person> ps = dt.GetEntityListByTable<Person>();
            Stopwatch s = new Stopwatch();
            s.Start();
            foreach (DataRow dr in dt.Rows)
            {

            }
            s.Stop();
            Console.WriteLine(s.ElapsedMilliseconds);
            Console.ReadLine();

            #endregion

        }

        private static void TestReflectProps<T>(T t)
        {
            foreach (PropertyInfo p in t.GetType().GetProperties())
            {
                Console.WriteLine("Name:{0} Value:{1}", p.Name, p.GetValue(t));
            }
        }

        private static void TestRandomNum()
        {
            Console.WriteLine("同时生成10个随机数");
            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(rand.Next(1, 10));
            }

            List<int> res = new List<int>();
            Console.WriteLine("生成10个1-66之间的随机数");
            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(1, 66));
            }

            Console.WriteLine(string.Join(",", res));

            res.Clear();
            RandomNumberCore.InitRandomArray();
            Console.WriteLine("初始化随机数组后再次生成10个1-66之间的随机数");
            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(1, 66));
            }
            Console.WriteLine(string.Join(",", res));
            res.Clear();
            Console.WriteLine("生成10个67-18239之间的随机数");
            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(67, 18239));
            }
            Console.WriteLine(string.Join(",", res));

            Console.ReadKey();
        }

        /// <summary>
        /// 获取时间
        /// </summary>
        /// <returns></returns>
        private static string GetCurrDate()
        {
            return DateTime.Now.ToString();
        }

        private static DataTable GetDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("id");
            dt.Columns.Add("name");

            for (int i = 0; i < 10000; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = i;
                dr[1] = Guid.NewGuid().ToString();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        [Serializable]
        public class LocalDBTest : DOBase
        {
            public string Name { get; set; }

            public int Age { get; set; }

            public List<string> remark { get; set; }

            public List<T2> T2S { get; set; }
        }

        [Serializable]
        public class T2 : DOBase
        {
            public string T2Name { get; set; }
        }

        public class Person
        {
            public string id { get; set; }

            public int Int_id
            {
                get { return id.TryChangeType(0); }
            }
            public string Name { get; set; }

            private static readonly List<Person> allSource;
        }

        public class Group
        {
            public int GroupID { get; set; }

            public string GroupName { get; set; }

            public List<Person> PS { get; set; }

            public string Mark { get; set; }
        }


        public class DynamicObjectTest
        {
            public dynamic LocalCache { get; private set; }

            public DynamicObjectTest()
            {
                LocalCache = new ExpandoObject();
            }
        }

        /// <summary>
        /// 本地缓存容器
        /// </summary>
        class LocalCacheContainer : DynamicObject
        {
            readonly Dictionary<string, object> _dic = new Dictionary<string, object>();

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var name = binder.Name;
                _dic.TryGetValue(name, out result);
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                var name = binder.Name;
                _dic[name] = value;
                return true;
            }
        }

        public class RTest
        {
            public int i = 0;
        }

        public class InnerC
        {
            public void T(RTest t)
            {
                t.i = 10;
            }

            public void T(ref RTest t)
            {
                t.i = 100;
            }
        }
    }
}
