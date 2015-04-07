using System;
using System.Collections.Generic;
using System.Timers;

namespace DeanZhou.Framework
{
    public class DataBufferPool<TItem>
    {
        private readonly Queue<TItem> ItemsQueue;

        private readonly Action<List<T>> ExecAction;

        private static readonly object _lockObj = new object();

        private static readonly object _lockExecObj = new object();
         

        public DataBufferPool(Action<TItem> execAction)
        {
            ItemsQueue = new Queue<TItem>();

            Timer autoTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = 5000
            };
            autoTimer.Elapsed += autoTimer_Elapsed;
            ExecAction = execAction;
        }

        void autoTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<TItem> ls = new List<TItem>();
            lock (_lockObj)
            {
                while (ItemsQueue.Count > 0)
                {
                    ls.Add(ItemsQueue.Dequeue());
                }
            }

            lock (_lockExecObj)
            {
                foreach (TItem item in ls)
                {
                    ExecAction(item);
                }
            }
            ls.Clear();
        }

        public void AddItem(TItem item)
        {
            lock (_lockObj)
            {
                ItemsQueue.Enqueue(item);
            }
        }
    }
}
