﻿using System;
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
        private static readonly object LockObj = new object();

        /// <summary>
        /// 数据暂存队列
        /// </summary>
        private readonly Queue<TItem> _itemsQueue;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="execAction">处理批量数据</param>
        /// <param name="interval">暂存时间 毫秒</param>
        public DataBufferPool(Action<List<TItem>> execAction, double interval = 5000)
        {
            //初始化队列
            _itemsQueue = new Queue<TItem>();

            //定时器 定时处理缓存的数据
            Timer autoTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = interval
            };
            autoTimer.Elapsed += (sender, e) =>
            {
                try
                {
                    List<TItem> ls = new List<TItem>();
                    lock (LockObj)
                    {
                        while (_itemsQueue.Count > 0)
                        {
                            ls.Add(_itemsQueue.Dequeue());
                        }
                    }
                    if (ls.Count > 0 && execAction != null)
                    {
                        execAction(ls);
                    }
                    ls.Clear();
                }
                catch (Exception)
                {
                }
            };
            autoTimer.Start();
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
    }
}
