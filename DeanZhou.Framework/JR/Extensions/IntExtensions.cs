using System;
using System.Collections.Generic;
/// <summary>
/// 	Extension methods for the string data type
/// </summary>
public static class IntExtensions
{
    /// <summary>
    /// 	Performs the specified action n times based on the underlying int value.
    /// </summary>
    /// <param name = "value">The value.</param>
    /// <param name = "action">The action.</param>
    public static void Times(this int value, Action action)
    {
        for (var i = 0; i < value; i++)
            action();
    }

    /// <summary>
    /// 	Performs the specified action n times based on the underlying int value.
    /// </summary>
    /// <param name = "value">The value.</param>
    /// <param name = "action">The action.</param>
    public static void Times(this int value, Action<int> action)
    {
        for (var i = 0; i < value; i++)
            action(i);
    }

    /// <summary>
    /// 	Performs the specified action n times based on the underlying int value.
    /// </summary>
    /// <param name = "times">The value.</param>
    /// <param name = "action">The action.</param>
    /// <returns>action返回值集合</returns>
    public static List<T> Times<T>(this int times, Func<T> action)
    {
        var list = new List<T>();
        for (var i = 0; i < times; i++)
        {
            list.Add(action());
        }
        return list;
    }
    /// <summary>
    /// Performs the specified action n times based on the underlying int value.
    /// </summary>
    /// <param name = "times">重复次数</param>
    /// <param name = "action">The action.</param>
    /// <returns>action返回值集合</returns>
    public static List<T> Times<T>(this int times, Func<int, T> action)
    {
        var list = new List<T>();
        for (var i = 0; i < times; i++)
        {
            list.Add(action(i));
        }
        return list;
    }
}