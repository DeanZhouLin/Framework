using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeanZhou.Framework
{
    //该类扩展ThreadPool，实现并发执行多个任务，且等待所有任务执行完成以后，再继续执行主线程
    /// <summary>
    /// 该类扩展ThreadPool，实现并发执行多个任务，且等待所有任务执行完成以后，再继续执行主线程
    /// </summary>
    public static class ThreadPoolExtension
    {
        /// <summary>
        /// 并发执行execActions，并等待所有的execActions都执行完成以后，再往下执行
        /// </summary>
        /// <param name="execActions">需要执行的方法</param>
        public static void ExecUserWorkItemsAndWaitAll(params Action[] execActions)
        {
            if (execActions == null || !execActions.Any())
            {
                return;
            }
            int execCount = execActions.Count();
            using (var countdown = new MutipleThreadResetEvent(execCount))
            {
                foreach (Action execAction in execActions)
                {
                    Action action = execAction;
                    ThreadPool.QueueUserWorkItem(x =>
                    {
                        action();
                        //发送信号量 本线程执行完毕
                        MutipleThreadResetEvent countdown1 = x as MutipleThreadResetEvent;
                        if (countdown1 != null) countdown1.SetOne();
                    }, countdown);
                }
                //等待所有线程执行完毕
                countdown.WaitAll();
            }
        }
    }
}
