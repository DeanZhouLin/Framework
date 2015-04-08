namespace DeanZhou.Framework
{
    /// <summary>
    /// 类型识别器接口
    /// </summary>
    public interface IItemTypeIdentifier<in TItem, in TParam, out TItemType>
        where TItem : class
        where TParam : class
    {
        /// <summary>
        /// 类型识别器（含参）
        /// </summary>
        /// <typeparam name="TItemType">待操作数据数据数据类型</typeparam>
        /// <typeparam name="TParam">辅助参数数据类型</typeparam>
        /// <typeparam name="TItemType">识别类型枚举</typeparam>
        /// <param name="item">待操作数据数据</param>
        /// <param name="param">辅助参数</param>
        /// <returns>识别类型</returns>
        TItemType IdentifyItemType(TItem item, TParam param);
    }
}
