using System;
using System.Collections.Generic;
using System.Timers;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 数据池
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class DataBufferPool<TItem>
    {
        /// <summary>
        /// 锁
        /// </summary>
        private static readonly object _lockObj = new object();

        /// <summary>
        /// 数据暂存队列
        /// </summary>
        private readonly Queue<TItem> ItemsQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execAction">处理批量数据</param>
        /// <param name="interval">暂存时间 毫秒</param>
        public DataBufferPool(Action<List<TItem>> execAction, double interval = 5000)
        {
            //初始化队列
            ItemsQueue = new Queue<TItem>();

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
                lock (_lockObj)
                {
                    while (ItemsQueue.Count > 0)
                    {
                        ls.Add(ItemsQueue.Dequeue());
                    }
                }
                if (ls.Count > 0 && execAction != null)
                {
                    execAction(ls);
                }
                ls.Clear();
            };
            autoTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void PushItem(TItem item)
        {
            lock (_lockObj)
            {
                ItemsQueue.Enqueue(item);
            }
        }
    }
}
