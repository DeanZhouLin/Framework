using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeanZhou.Framework.ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            #region 数据缓存仓库测试

            //key
            const string key = "GetCurrDateKey";
            //初始化仓库
            DataWarehouse<string>.InitDataItem(key, GetCurrDate, 1);

            //根据key获取值
            Console.WriteLine(DataWarehouse<string>.GetData(key));
            //休眠 等待过期
            Thread.Sleep(1000 * 61);
            //再次根据key获取值
            Console.WriteLine(DataWarehouse<string>.GetData(key));

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
    }
}
