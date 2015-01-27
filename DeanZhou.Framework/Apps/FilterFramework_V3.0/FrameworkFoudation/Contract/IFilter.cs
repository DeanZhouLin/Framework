namespace DeanZhou.Framework
{
    /// <summary>
    /// 过滤器接口
    /// </summary>
    public interface IFilter<in TItemType, in TParamType>
        where TItemType : class
        where TParamType : class
    {
        /// <summary>
        /// 执行过滤操作
        /// </summary>
        /// <param name="itemType">待过滤数据数据</param>
        /// <param name="paramType">辅助参数</param>
        /// <returns>是否通过过滤</returns>
        bool DoFilter(TItemType itemType, TParamType paramType);
    }

    public interface IFilter<in TItemType>
        where TItemType : class
    {
        /// <summary>
        /// 执行过滤操作
        /// </summary>
        /// <typeparam name="TItemType">待过滤数据数据</typeparam>
        /// <param name="itemType"></param>
        /// <returns>是否通过过滤</returns>
        bool DoFilter(TItemType itemType);

    }
}
