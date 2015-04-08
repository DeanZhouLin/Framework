using System;
using System.Collections.Generic;
using System.Timers;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 数据缓冲池
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class DataBufferPool<TItem>
    {
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object LockObj = new object();

        /// <summary>
        /// 数据缓冲池 名称
        /// </summary>
        public string DataBufferPoolName { get; set; }

        /// <summary>
        /// 数据暂存队列
        /// </summary>
        private readonly Queue<TItem> _itemsQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execAction">处理批量数据</param>
        /// <param name="interval">暂存时间 毫秒</param>
        /// <param name="poolName"></param>
        public DataBufferPool(Action<List<TItem>> execAction, double interval = 5000, string poolName = "DataBufferPool")
        {
            //实例化队列
            _itemsQueue = new Queue<TItem>();
            InitDataBufferPool(execAction, interval, poolName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(TItem item)
        {
            lock (LockObj)
            {
                _itemsQueue.Enqueue(item);
            }
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="execAction">处理批量数据</param>
        /// <param name="interval">暂存时间 毫秒</param>
        /// <param name="poolName"></param>
        private void InitDataBufferPool(Action<List<TItem>> execAction, double interval = 5000, string poolName = "DataBufferPool")
        {
            DataBufferPoolName = typeof(TItem).Name + "_" + poolName;

            _itemsQueue.Clear();

            //定时器 定时处理缓存的数据
            Timer autoTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = interval
            };
            autoTimer.Elapsed += (sender, e) =>
            {
                List<TItem> ls = new List<TItem>();
                lock (LockObj)
                {
                    while (_itemsQueue.Count > 0)
                    {
                        ls.Add(_itemsQueue.Dequeue());
                    }
                }

                LogHelper.CustomInfoEnabled = true;
                string log = string.Format("本次执行【{0}】条数据", ls.Count);
                LogHelper.CustomInfo(log, DataBufferPoolName);

                if (ls.Count > 0 && execAction != null)
                {
                    execAction(ls);
                }
                ls.Clear();
            };
            autoTimer.Start();
        }
    }
}
