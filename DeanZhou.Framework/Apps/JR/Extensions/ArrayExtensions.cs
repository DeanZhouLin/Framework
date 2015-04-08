using System;

/// <summary>
/// 扩展数组基类
/// </summary>
public static class ArrayExtensions
{
    ///<summary>
    ///	检查数组是否为空
    ///	<para>null || length==0 </para>
    ///</summary>
    ///<param name = "source">数组对象</param>
    ///<returns></returns>
    public static bool IsNullOrEmpty(this Array source)
    {
        if (source == null)
            return true;
        return source != null ? source.Length <= 0 : false;
    }

    ///<summary>
    ///	检查指定的索引号是否包含在数组中
    ///</summary>
    ///<param name = "source">数组对象</param>
    ///<param name = "index">索引号</param>
    ///<returns></returns>
    public static bool WithinIndex(this Array source, int index)
    {
        return source != null && index >= 0 && index < source.Length;
    }

    ///<summary>
    ///	检查指定的索引号是否包含在数组中
    ///</summary>
    ///<param name = "source">数组对象</param>
    ///<param name = "index">索引号</param>
    ///<param name="dimension">维度</param>
    ///<returns></returns>
    public static bool WithinIndex(this Array source, int index, int dimension = 0)
    {
        return source != null && index >= source.GetLowerBound(dimension) && index <= source.GetUpperBound(dimension);
    }
}