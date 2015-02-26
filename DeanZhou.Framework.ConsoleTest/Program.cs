using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace DeanZhou.Framework.ConsoleTest
{

    class Program
    {
        static void Main(string[] args)
        {

            //TestRandomNum();

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

        private static void TestRandomNum()
        {
            List<int> res = new List<int>();
            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(1, 66));
            }

            Console.WriteLine(string.Join(",", res));

            res.Clear();
            RandomNumberCore.InitRandomArray();
            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(1, 66));
            }
            Console.WriteLine(string.Join(",", res));
            res.Clear();

            for (int i = 0; i < 10; i++)
            {
                res.Add(RandomNumberCore.GetRandomNumber(67, 189));
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
                get { return id.ChangeType<int>(); }
            }
            public string Name { get; set; }
        }
    }
}
