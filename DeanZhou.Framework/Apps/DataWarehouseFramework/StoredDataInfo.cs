using System;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 存储的数据结构
    /// </summary>
    /// <typeparam name="T">需要存储的数据的数据类型（string int ..）</typeparam>
    public sealed class StoredDataInfo<T>
    {

        /// <summary>
        /// 存储的数据
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// 获取需要存储的数据的方法
        /// </summary>
        public Func<T> GetDataMethod { get; set; }

        /// <summary>
        /// 数据过期的时间
        /// </summary>
        public int TimeOfDuration { get; set; }

        /// <summary>
        /// 数据上一次被更新的时间
        /// </summary>
        public DateTime LastModifyTime { get; set; }

    }
}
