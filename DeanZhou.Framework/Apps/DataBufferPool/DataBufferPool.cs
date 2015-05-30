using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

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
        private static readonly object LockObj = default(TItem);

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
        public DataBufferPool(Action<List<TItem>> execAction, int interval = 5000, string poolName = "DataBufferPool")
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
        private void InitDataBufferPool(Action<List<TItem>> execAction, int interval = 5000, string poolName = "DataBufferPool")
        {
            DataBufferPoolName = typeof(TItem).Name + "_" + poolName;

            _itemsQueue.Clear();

            AutoClock.Regist(() =>
            {
                List<TItem> ls = null;
                lock (LockObj)
                {
                    while (_itemsQueue.Count > 0)
                    {
                        if (ls == null)
                        {
                            ls = new List<TItem>();
                        }
                        ls.Add(_itemsQueue.Dequeue());
                    }
                }

                LogHelper.CustomInfoEnabled = true;
                string log = string.Format("本次执行【{0}】条数据", ls == null ? "null" : ls.Count.ToString());
                LogHelper.CustomInfo(log, DataBufferPoolName);

                if (ls != null && ls.Count > 0 && execAction != null)
                {
                    execAction(ls);
                    ls.Clear();
                }
            }, interval);
        }
    }

    public class AsyncWorker<TItem>
    {
        public int WorkStepSeconds { get; set; }

        public Action<List<TItem>> Worker { get; set; }

        public ConcurrentQueue<TItem> Datas { get; set; }

        private Task t;
        private bool stopped;

        public AsyncWorker(int workStepSeconds, Action<List<TItem>> worker)
        {
            WorkStepSeconds = workStepSeconds;
            Worker = worker;
            Datas = new ConcurrentQueue<TItem>();
            Start();
        }

        private void Start()
        {
            t = new Task(DoAsyncWorker);
            t.Start();
        }

        public void AddItem(TItem data)
        {
            Datas.Enqueue(data);
        }

        public void StopAndWait()
        {
            stopped = true;
            t.Wait();
        }

        private void DoAsyncWorker()
        {
            List<TItem> ls = new List<TItem>();
            while (!stopped)
            {
                TItem data;
                while (Datas.TryDequeue(out data))
                {
                    ls.Add(data);
                }
                Worker(ls);
                ls.Clear();
                Thread.Sleep(WorkStepSeconds * 1000);
            }
        }
    }
}
