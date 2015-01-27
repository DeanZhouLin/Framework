using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace DeanZhou.Framework.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var res = StringFilterCore.NewInstance().Test();


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
