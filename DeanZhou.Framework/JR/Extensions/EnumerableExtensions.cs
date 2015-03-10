using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 	Extension methods for all kinds of (typed) enumerable data (Array, List, ...)
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// 	Converts all items of a list and returns them as enumerable.
    /// </summary>
    /// <typeparam name = "TSource">The source data type</typeparam>
    /// <typeparam name = "TTarget">The target data type</typeparam>
    /// <param name = "source">The source data.</param>
    /// <returns>The converted data</returns>
    /// <example>
    /// 	var values = new[] { "1", "2", "3" };
    /// 	values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
    /// </example>
    public static IEnumerable<TTarget> ConvertList<TSource, TTarget>(this IEnumerable<TSource> source)
    {
        return source.Select(value => value.ConvertTo<TTarget>());
    }

    /// <summary>
    /// 	Performs an action for each item in the enumerable
    /// </summary>
    /// <typeparam name = "T">The enumerable data type</typeparam>
    /// <param name = "values">The data values.</param>
    /// <param name = "action">The action to be performed.</param>
    /// <example>
    /// 	var values = new[] { "1", "2", "3" };
    /// 	values.ConvertList&lt;string, int&gt;().ForEach(Console.WriteLine);
    /// </example>
    /// <remarks>
    /// 	This method was intended to return the passed values to provide method chaining. Howver due to defered execution the compiler would actually never run the entire code at all.
    /// </remarks>
    public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
    {
        foreach (var value in values)
            action(value);
    }

    ///<summary>
    ///	Returns enumerable object based on target, which does not contains null references.
    ///	If target is null reference, returns empty enumerable object.
    ///</summary>
    ///<typeparam name = "T">Type of items in target.</typeparam>
    ///<param name = "target">Target enumerable object. Can be null.</param>
    ///<example>
    ///	object[] items = null;
    ///	foreach(var item in items.NotNull()){
    ///	// result of items.NotNull() is empty but not null enumerable
    ///	}
    /// 
    ///	object[] items = new object[]{ null, "Hello World!", null, "Good bye!" };
    ///	foreach(var item in items.NotNull()){
    ///	// result of items.NotNull() is enumerable with two strings
    ///	}
    ///</example>
    ///<remarks>
    ///	Contributed by tencokacistromy, http://www.codeplex.com/site/users/view/tencokacistromy
    ///</remarks>
    public static IEnumerable<T> IgnoreNulls<T>(this IEnumerable<T> target)
    {
        if (ReferenceEquals(target, null))
            yield break;

        foreach (var item in target.Where(item => !ReferenceEquals(item, null)))
            yield return item;
    }

    ///<summary>
    ///	Get Distinct
    ///</summary>
    ///<param name = "source"></param>
    ///<param name = "expression"></param>
    ///<typeparam name = "T"></typeparam>
    ///<typeparam name = "TKey"></typeparam>
    ///<returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> expression)
    {
        return source == null ? Enumerable.Empty<T>() : source.GroupBy(expression).Select(i => i.First());
    }

    ///<summary>
    ///	Remove item from a list
    ///</summary>
    ///<param name = "source"></param>
    ///<param name = "predicate"></param>
    ///<typeparam name = "T"></typeparam>
    ///<returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Predicate<T> predicate)
    {
        if (source == null)
            return Enumerable.Empty<T>();

        var list = source.ToList();
        list.RemoveAll(predicate);
        return list;
    }
}
