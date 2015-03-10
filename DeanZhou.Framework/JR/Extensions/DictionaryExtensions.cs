using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

/// <summary>
/// Dictionary扩展类
/// </summary>
public static class DictionaryExtensions
{
    /// <summary>
    /// 获取指定Key的值，不存在则返回默认值
    /// </summary>
    /// <typeparam name="TKey">集合Key类型</typeparam>
    /// <typeparam name="TValue">集合Value类型</typeparam>
    /// <param name="dictionary">IDictionary</param>
    /// <param name="key">Key</param>
    /// <param name="defaultValue">defaultValue</param>
    /// <returns>指定Key的值，不存在则返回默认值</returns>
    public static TValue GetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue = default(TValue))
    {
        return dictionary.ContainsKey(key) ? dictionary[key] : defaultValue;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="dictionary"></param>
    /// <param name="onEachFn"></param>
    public static void ForEach<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Action<TKey, TValue> onEachFn)
    {
        foreach (var entry in dictionary)
        {
            onEachFn(entry.Key, entry.Value);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="map"></param>
    /// <param name="createFn"></param>
    /// <returns></returns>
    public static List<T> ConvertAll<T, K, V>(IDictionary<K, V> map, Func<K, V, T> createFn)
    {
        var list = new List<T>();
        map.ForEach((kvp) => list.Add(createFn(kvp.Key, kvp.Value)));
        return list;
    }

    /// <summary>
    /// 指定的Key存在则修改，不存在则新增
    /// </summary>
    /// <param name="dic">IDictionary</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value</param>
    public static void AddOrUpdate<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
    {
        if (dic.ContainsKey(key))
        {
            dic[key] = value;
        }
        else
        {
            dic.Add(key, value);
        }
    }
}
