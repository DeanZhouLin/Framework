using System;

namespace DeanZhou.Framework
{
    [Flags]
    public enum DemoStringEnumType
    {
        /// <summary>
        /// 长度大于3
        /// </summary>
        LongLen = 1,

        /// <summary>
        /// 包含1
        /// </summary>
        HasOne = 2, 

        /// <summary>
        /// 无效值
        /// </summary>
        NN = 1024
    }

    [Flags]
    public enum StringEnumType
    {
        /// <summary>
        /// 字符串长度小于5
        /// </summary>
        LongLen = 1,

        /// <summary>
        /// 字符串长度大于20
        /// </summary>
        ShortLen = 2,

        /// <summary>
        /// 字符串长度大于5 小于20
        /// </summary>
        MiddleLen = 4,

        /// <summary>
        /// 字符串为空
        /// </summary>
        Empty = 8,

        /// <summary>
        /// 纯数字
        /// </summary>
        Number = 16,

        /// <summary>
        /// 无效值
        /// </summary>
        NN = 1024
    }
}
