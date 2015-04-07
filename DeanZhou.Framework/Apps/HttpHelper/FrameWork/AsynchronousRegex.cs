using System;
using System.Text.RegularExpressions;
using System.Threading;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 异步的正则匹配类
    /// 解决匹配过程中产生的无限回朔问题
    /// </summary>
    public class AsynchronousRegex
    {
        private MatchCollection _mc;
        private readonly int _timeout;        // 最长休眠时间(超时),毫秒
        private int _sleepCounter;
        private readonly int _sleepInterval;    // 休眠间隔,毫秒

        public bool IsTimeout { get; private set; }

        public AsynchronousRegex(int timeout)
        {
            _timeout = timeout;
            _sleepCounter = 0;
            _sleepInterval = 100;
            IsTimeout = false;
            _mc = null;
        }

        public MatchCollection Matchs(Regex regex, string input)
        {
            var r = new Reg(regex, input);
            r.OnMatchComplete += MatchCompleteHandler;

            var t = new Thread(r.Matchs);
            t.Start();
            Sleep(t);
            return _mc;
        }

        private void Sleep(Thread t)
        {
            while (true)
            {
                if (t == null || !t.IsAlive) return;
                Thread.Sleep(TimeSpan.FromMilliseconds(_sleepInterval));
                _sleepCounter++;
                if (_sleepCounter * _sleepInterval >= _timeout)
                {
                    t.Abort();
                    IsTimeout = true;
                }
                else
                {
                    continue;
                }
                break;
            }
        }

        private void MatchCompleteHandler(MatchCollection matchCollection)
        {
            _mc = matchCollection;
        }

        class Reg
        {
            internal delegate void MatchCompleteHandler(MatchCollection mc);
            internal event MatchCompleteHandler OnMatchComplete;
            public Reg(Regex regex, string input)
            {
                Regex = regex;
                Input = input;
            }

            private string Input { get; set; }
            private Regex Regex { get; set; }
            internal void Matchs()
            {
                var mc = Regex.Matches(Input);
                if (mc.Count > 0)    // 这里有可能造成cpu资源耗尽
                {
                    OnMatchComplete(mc);
                }
            }
        }

    }
}
