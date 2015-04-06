using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using DeanZhou.Framework;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

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

            #region 过滤器测试

            //初始化过滤器
            ComplexFilterCore<string, DemoStringEnumType> demoStrFilterCore = new ComplexFilterCore<string, DemoStringEnumType>();

            //初始化 HasOne 2 LongLen 1
            demoStrFilterCore.SetMinGetCount(DemoStringEnumType.LongLen, 1);
            demoStrFilterCore.SetMinGetCount(DemoStringEnumType.HasOne, 2);

            //注册类型识别器 识别特性 DemoStringEnumType
            demoStrFilterCore.RegistEnumTypeIdentifier(new IdentifyDemoString());

            //添加过滤器 过滤掉不是整数的字符串
            demoStrFilterCore.AddFilter(new DemoStringContentFilter());

            //初始化数据源
            var ls = new List<string> { "123", "246", "b", "15", "16", "a32" };

            //执行过滤
            var result = demoStrFilterCore.GetFilteredResult(ls);

            //打印结果
            Console.WriteLine(string.Join(",", result.Keys));


            Console.ReadKey();

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

        public class Person
        {

            public string id { get; set; }

            public int Int_id
            {
                get { return id.TryChangeType(0); }
            }
            public string Name { get; set; }
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
