using System;

namespace DeanZhou.Framework
{
    [Flags]
    public enum EnumType
    {
        /// <summary>
        /// 适用于高票面价(Day 0 2)
        /// </summary>
        HighApply = 1,

        /// <summary>
        /// 适用于低票面价(Day 1 2)
        /// </summary>
        LowApply = 2,

        /// <summary>
        /// 当期适用政策(StartDate EndDate 在当前时间范围内)
        /// </summary>
        CurrentUse = 4,

        /// <summary>
        /// 下期适用政策（StartDate EndDate 在当前时间范围14天内）
        /// </summary>
        NextUse = 8,

        /// <summary>
        /// 自动出票供应商
        /// </summary>
        AutoTicket = 16
    }
}
