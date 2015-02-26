namespace DeanZhou.Framework
{
    /// <summary>
    /// 识别器
    /// 继承IEnumTypeIdentifier接口
    /// </summary>
    public class IdentifyDemoString : IEnumTypeIdentifier<string, DemoStringEnumType>
    {

        /// <summary>
        /// 识别对象特性
        /// </summary>
        /// <param name="item">待识别字符串</param>
        /// <returns>DemoStringEnumType</returns>
        public DemoStringEnumType IdentifyItemTypeAsEnumType(string item)
        {
            //初始化结果
            DemoStringEnumType result = DemoStringEnumType.NN;

            //识别长度是否大于3
            if (item.Length > 3)
            {
                result |= DemoStringEnumType.LongLen;
            }

            //识别是否包含1
            if (item.Contains("1"))
            {
                result |= DemoStringEnumType.HasOne;
            }

            return result;
        }

    }
}
