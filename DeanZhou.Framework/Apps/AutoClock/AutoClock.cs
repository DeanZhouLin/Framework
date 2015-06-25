using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Timers;

namespace DeanZhou.Framework
{
    public static class AutoClock
    {
        private static readonly ConcurrentDictionary<int, Action> WaitExecActions = new ConcurrentDictionary<int, Action>();

        private static readonly object _lockObj = new object();
        //定时器 定时处理缓存的数据
        static readonly Timer autoTimer = new Timer
         {
             AutoReset = true,
             Enabled = true,
             Interval = 1000
         };

        private static int rc;

        static AutoClock()
        {
            autoTimer.Elapsed += (sender, e) =>
            {
                Do();
            };
            autoTimer.Start();
        }

        public static void Start()
        {
            autoTimer.Start();
        }

        public static void Stop()
        {
            lock (_lockObj)
            {
                autoTimer.Stop();
            }
        }

        public static void Regist(Action action, int runInterval)
        {
            lock (_lockObj)
            {
                if (WaitExecActions.ContainsKey(runInterval))
                {
                    WaitExecActions[runInterval] += action;
                }
                else
                {
                    WaitExecActions.AddOrUpdate(runInterval, action);
                }
            }
        }

        private static void Do()
        {
            lock (_lockObj)
            {
                rc++;
                if (rc > WaitExecActions.Max(c => c.Key))
                {
                    rc = 1;
                }
                foreach (var waitExecAction in WaitExecActions)
                {
                    if (rc % waitExecAction.Key == 0)
                    {
                        waitExecAction.Value();
                    }
                }
            }
        }
    }
}
