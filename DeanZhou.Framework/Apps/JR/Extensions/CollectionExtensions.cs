using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 对ICollection的扩展
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// 向集合中添加一项，如果这项值已存在则不加到集合中同时返回false,如果这项值不存在则加入到集合中同时返回true
    /// </summary>
    /// <typeparam name = "T">泛型集合的值的类型</typeparam>
    /// <param name = "collection">集合对象的实例</param>
    /// <param name = "value">待添加的项</param>
    /// <returns>新增成功返回true,否则返回false</returns>
    /// <example>
    /// 	<code>
    /// 		list.AddUnique(1); // returns true;
    /// 		list.AddUnique(1); // returns false;
    /// 	</code>
    /// </example>
    public static bool AddUnique<T>(this ICollection<T> collection, T value)
    {
        if (collection.Contains(value) == false)
        {
            collection.Add(value);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 向集合中添加一组项，如果项已存在则不添加
    /// </summary>
    /// <typeparam name = "T">泛型集合的值的类型</typeparam>
    /// <param name = "collection">集合对象的实例</param>
    /// <param name = "values">待添加的一组项</param>
    /// <returns>返回成功添加的数量</returns>
    public static int AddRangeUnique<T>(this ICollection<T> collection, IEnumerable<T> values)
    {
        var count = 0;
        foreach (var value in values)
        {
            if (collection.AddUnique(value))
                count++;
        }
        return count;
    }

    /// <summary>
    /// 移除集合中所有符合条件的项
    /// </summary>
    /// <typeparam name="T">泛型集合的值的类型</typeparam>
    /// <param name="collection">集合对象的实例</param>
    /// <param name="predicate">表示定义一组条件并确定指定对象是否符合这些条件的方法</param>
    public static void RemoveAll<T>(this ICollection<T> collection, Predicate<T> predicate)
    {
        if (collection == null)
            throw new ArgumentNullException("collection");
        var deleteList = collection.Where(child => predicate(child)).ToList();
        deleteList.ForEach(t => collection.Remove(t));
    }
}
