namespace DeanZhou.Framework
{
    /// <summary>
    /// 退出类型
    /// </summary>
    public enum FinishedType
    {
        /// <summary>
        /// 超过最大检测条数退出
        /// </summary>
        CheckCount,

        /// <summary>
        /// 获得所有需要的类型退出
        /// </summary>
        NormalFinished,

        /// <summary>
        /// 自定义方法退出
        /// </summary>
        CustomerFinished
    }
}
