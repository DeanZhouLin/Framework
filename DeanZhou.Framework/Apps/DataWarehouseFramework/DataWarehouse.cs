using System;
using System.Collections.Generic;
using System.Linq;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 数据缓存仓库
    /// </summary>
    /// <typeparam name="T">需要存储的数据的数据类型</typeparam>
    public static class DataWarehouse<T>
    {
        //存储所有数据
        /// <summary>
        /// 存储所有数据
        /// </summary>
        private static readonly Dictionary<string, StoredDataInfo<T>> EntireStoredData = new Dictionary<string, StoredDataInfo<T>>();

        //锁
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object lockObj = new object();

        //初始化数据项
        /// <summary>
        /// 初始化数据项
        /// </summary>
        /// <param name="key">查找key</param>
        /// <param name="getDataMethod">获取数据方法</param>
        /// <param name="timeOfDuration">过期时间（分钟）</param>
        public static string InitDataItem(string key, Func<T> getDataMethod, int timeOfDuration)
        {
            if (HasKey(key))
            {
                return "key:" + key + " 已存在";
            }
            return InitStoredDataItem(key, new StoredDataInfo<T>
            {
                GetDataMethod = getDataMethod,
                TimeOfDuration = timeOfDuration,
                LastModifyTime = DateTime.Now.AddMinutes(-2 * timeOfDuration)
            });
        }

            //初始化数据项
            /// <summary>
            /// 初始化数据项
            /// </summary>
            /// <param name="key"></param>
            /// <param name="storedData"></param>
            private static string InitStoredDataItem(string key, StoredDataInfo<T> storedData)
            {
                lock (lockObj)
                {
                    if (EntireStoredData.ContainsKey(key))
                    {
                        return "key:" + key + " 已存在";
                    }
                    EntireStoredData.Add(key, storedData);
                }
                return "";
            }

        // 获取指定key的数据项
        /// <summary>
        /// 获取指定key的数据项
        /// </summary>
        /// <param name="key"></param>
        /// <param name="isForcedRefresh">是否强制更新</param>
        /// <returns></returns>
        public static T GetData(string key, bool isForcedRefresh = false)
        {
            //不存在key
            if (!HasKey(key))
            {
                #region

                string currKeys = "";
                string currTType = "";
                if (EntireStoredData.Any())
                {
                    currKeys = string.Join(",", EntireStoredData.Keys.ToArray());
                    var v = EntireStoredData.First().Value.Data;
                    currTType = v.GetType().ToString();
                }
                throw new Exception(string.Format("无指定key：{0}，当前池包含key集合{1}，当前池类型：{2}", key, currKeys, currTType));

                #endregion
            }

            //根据key获取value
            StoredDataInfo<T> sdi = EntireStoredData[key];

            //判断是否过期
            int timeOfDuration = sdi.TimeOfDuration;
            DateTime lastModifyTime = sdi.LastModifyTime;

            if (!isForcedRefresh && DateTime.Now.AddMinutes(-timeOfDuration) <= lastModifyTime)
                return sdi.Data;

            //重新更新数据
            sdi.Data = sdi.GetDataMethod();
            sdi.LastModifyTime = DateTime.Now;

            return sdi.Data;
        }

        //强制刷新数据
        /// <summary>
        /// 强制刷新数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T RefreshData(string key)
        {
            try
            {
                return GetData(key, true);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        //是否已经包含key
        /// <summary>
        /// 是否已经包含key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            lock (lockObj)
            {
                return EntireStoredData.ContainsKey(key);
            }
        }

        //当前所有key的列表
        /// <summary>
        /// 当前所有key的列表
        /// </summary>
        public static string[] CurrKeysArray
        {
            get
            {
                return EntireStoredData.Keys.ToArray();
            }
        }

        //当前所有key的字符串显示
        /// <summary>
        /// 当前所有key的字符串显示
        /// </summary>
        public static string CurrKeys
        {
            get
            {
                return string.Join(",", CurrKeysArray);
            }
        }

    }
}
