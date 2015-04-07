using System;
using System.Threading;

namespace DeanZhou.Framework
{
    //封装ManualResetEvent
    /// <summary>
    ///  封装ManualResetEvent
    /// </summary>
    internal sealed class MutipleThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent done;
        private long current;

        //构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="total">需要等待执行的线程总数</param>
        public MutipleThreadResetEvent(int total)
        {
            current = total;
            done = new ManualResetEvent(false);
        }

        //唤醒一个等待的线程
        /// <summary>
        /// 唤醒一个等待的线程
        /// </summary>
        public void SetOne()
        {
            // Interlocked 原子操作类 ,此处将计数器减1
            if (Interlocked.Decrement(ref current) == 0)
            {
                //当所以等待线程执行完毕时，唤醒等待的线程
                done.Set();
            }
        }

        //等待所以线程执行完毕
        /// <summary>
        /// 等待所以线程执行完毕
        /// </summary>
        public void WaitAll()
        {
            done.WaitOne();
        }

        //释放对象占用的空间
        /// <summary>
        /// 释放对象占用的空间
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)done).Dispose();
        }
    }
}
