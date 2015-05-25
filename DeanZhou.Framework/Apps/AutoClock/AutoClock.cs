using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace DeanZhou.Framework
{
    public static class AutoClock
    {
        private static readonly Dictionary<int, Action> WaitExecActions = new Dictionary<int, Action>();

        private static readonly object _lockObj = new object();

        private static int rc;

        static AutoClock()
        {
            //定时器 定时处理缓存的数据
            Timer autoTimer = new Timer
            {
                AutoReset = true,
                Enabled = true,
                Interval = 1000
            };
            autoTimer.Elapsed += (sender, e) =>
            {
                Do();
            };
            autoTimer.Start();
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
                    WaitExecActions.Add(runInterval, action);
                }
            }
        }

        public static void Do()
        {
            lock (_lockObj)
            {
                rc++;
                if (rc > WaitExecActions.Max(c => c.Key))
                {
                    rc = 1;
                }
                if (WaitExecActions.ContainsKey(rc))
                {
                    WaitExecActions[rc]();
                }
            }
        }
    }
}
