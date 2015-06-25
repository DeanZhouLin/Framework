using System;

namespace DeanZhou.Framework.Apps
{
    [Serializable]
    public class CheckData : DOBase
    {
        public string CheckResult { get; set; }

        public DateTime CheckTime { get; set; }

    }
}
