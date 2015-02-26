namespace DeanZhou.Framework
{
    
    /// <summary>
    /// 是否是整数过滤器
    /// 继承IFilter
    /// </summary>
    public class IsNumberFilter : IFilter<string>
    {

        /// <summary>
        /// 判断字符串是否为政策
        /// </summary>
        /// <param name="item">待检验字符串</param>
        /// <returns>bool</returns>
        public bool DoFilter(string item)
        {
            int tem; 
            return int.TryParse(item, out tem);
        }
    }
}
