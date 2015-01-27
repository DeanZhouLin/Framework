namespace DeanZhou.Framework
{
    /// <summary>
    /// 类型识别器接口
    /// </summary>
    public interface IEnumTypeIdentifier<in TItemType, in TParamType, out TEnumType>
        where TItemType : class
        where TParamType : class
    {
        /// <summary>
        /// 类型识别器（含参）
        /// </summary>
        /// <typeparam name="TItemType">待操作数据数据数据类型</typeparam>
        /// <typeparam name="TParamType">辅助参数数据类型</typeparam>
        /// <typeparam name="TEnumType">识别类型枚举</typeparam>
        /// <param name="itemType">待操作数据数据</param>
        /// <param name="paramType">辅助参数</param>
        /// <returns>识别类型</returns>
        TEnumType IdentifyItemTypeAsEnumType(TItemType itemType, TParamType paramType);
    }

    /// <summary>
    /// 类型识别器接口
    /// </summary>
    public interface IEnumTypeIdentifier<in TItemType, out TEnumType>
          where TItemType : class
    {
        /// <summary>
        /// 类型识别器（无参）
        /// </summary>
        /// <typeparam name="TItemType">待操作数据数据数据类型</typeparam>
        /// <typeparam name="TEnumType">识别类型枚举</typeparam>
        /// <param name="itemType">待操作数据数据</param>
        /// <returns>识别类型</returns>
        TEnumType IdentifyItemTypeAsEnumType(TItemType itemType);
    }
}
