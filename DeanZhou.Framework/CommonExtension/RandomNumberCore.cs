using System;

namespace DeanZhou.Framework
{
    public class RandomNumberCore
    {

        /// <summary>
        /// 锁
        /// </summary>
        static readonly object _lockObj;

        /// <summary>
        /// 随机256个byte数字
        /// </summary>
        static readonly byte[] _staticNumArr;

        /// <summary>
        /// 取值索引
        /// </summary>
        static int _staticNumIndex;

        /// <summary>
        /// 静态构造
        /// </summary>
        static RandomNumberCore()
        {
            //初始化锁
            _lockObj = new object();

            //初始化索引
            _staticNumIndex = 0;

            //获取随机数字数组
            _staticNumArr = new byte[256];
            new Random().NextBytes(_staticNumArr);
        }

        /// <summary>
        /// 获取随机数
        /// </summary>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        /// <returns>满足范围的随机数</returns>
        public static int GetRandomNumber(int minValue, int maxValue)
        {
            //保证输入大于0
            if (maxValue < 1)
            {
                maxValue = 1;
            }

            if (minValue < 1)
            {
                minValue = 1;
            }

            lock (_lockObj)
            {
                //当前取随机数索引递增
                _staticNumIndex++;

                //获取有效索引
                _staticNumIndex = _staticNumIndex % 255;

                //从随机数组中取值
                int tem = _staticNumArr[_staticNumIndex] % maxValue;

                //随机数映射
                int res = (int)((tem / 256.0) * (maxValue - minValue)) + minValue;
                return res;
            }
        }

        /// <summary>
        /// 初始化随机数组
        /// </summary>
        public static void InitRandomArray()
        {
            new Random().NextBytes(_staticNumArr);
        }
    }
}
