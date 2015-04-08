namespace DeanZhou.Framework
{
    /// <summary>
    /// 过滤器接口
    /// </summary>
    public interface IFilter<in TItem, in TItemType>
        where TItem : class
        where TItemType : class
    {
        /// <summary>
        /// 执行过滤操作
        /// </summary>
        /// <param name="item">待过滤数据数据</param>
        /// <param name="param">辅助参数</param>
        /// <returns>是否通过过滤</returns>
        bool DoFilter(TItem item, TItemType param);
    }
}
