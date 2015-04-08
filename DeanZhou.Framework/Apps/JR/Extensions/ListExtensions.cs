using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

/// <summary>
/// 	Extension methods for all kind of Lists implementing the IList&lt;T&gt; interface
/// </summary>
public static class ListExtensions
{
    /// <summary>
    /// 	Inserts an item uniquely to to a list and returns a value whether the item was inserted or not.
    /// </summary>
    /// <typeparam name = "T">The generic list item type.</typeparam>
    /// <param name = "list">The list to be inserted into.</param>
    /// <param name = "index">The index to insert the item at.</param>
    /// <param name = "item">The item to be added.</param>
    /// <returns>Indicates whether the item was inserted or not</returns>
    public static bool InsertUnqiue<T>(this IList<T> list, int index, T item)
    {
        if (list.Contains(item) == false)
        {
            list.Insert(index, item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 	Inserts a range of items uniquely to a list starting at a given index and returns the amount of items inserted.
    /// </summary>
    /// <typeparam name = "T">The generic list item type.</typeparam>
    /// <param name = "list">The list to be inserted into.</param>
    /// <param name = "startIndex">The start index.</param>
    /// <param name = "items">The items to be inserted.</param>
    /// <returns>The amount if items that were inserted.</returns>
    public static int InsertRangeUnique<T>(this IList<T> list, int startIndex, IEnumerable<T> items)
    {
        var index = startIndex + items.Count(item => list.InsertUnqiue(startIndex, item));
        return (index - startIndex);
    }

    /// <summary>
    /// 	Return the index of the first matching item or -1.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "list">The list.</param>
    /// <param name = "comparison">The comparison.</param>
    /// <returns>The item index</returns>
    public static int IndexOf<T>(this IList<T> list, Func<T, bool> comparison)
    {
        for (var i = 0; i < list.Count; i++)
        {
            if (comparison(list[i]))
                return i;
        }
        return -1;
    }

    /// <summary>
    /// 	Join all the elements in the list and create a string seperated by the specified char.
    /// </summary>
    /// <param name = "list">
    /// 	The list.
    /// </param>
    /// <param name = "joinChar">
    /// 	The join char.
    /// </param>
    /// <typeparam name = "T">
    /// </typeparam>
    /// <returns>
    /// 	The resulting string of the elements in the list.
    /// </returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static string Join<T>(this IList<T> list, char joinChar)
    {
        return list.Join(joinChar.ToString());
    }

    /// <summary>
    /// 	Join all the elements in the list and create a string seperated by the specified string.
    /// </summary>
    /// <param name = "list">
    /// 	The list.
    /// </param>
    /// <param name = "joinString">
    /// 	The join string.
    /// </param>
    /// <typeparam name = "T">
    /// </typeparam>
    /// <returns>
    /// 	The resulting string of the elements in the list.
    /// </returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// 	Optimised by Mario Majcica
    /// </remarks>
    public static string Join<T>(this IList<T> list, string joinString)
    {
        StringBuilder result = new StringBuilder();

        int listCount = list.Count;
        int listCountMinusOne = listCount - 1;

        if (list != null && listCount > 0)
        {
            if (listCount > 1)
            {
                for (var i = 0; i < listCount; i++)
                {
                    if (i != listCountMinusOne)
                    {
                        result.Append(list[i]);
                        result.Append(joinString);
                    }
                    else
                        result.Append(list[i]);
                }
            }
            else
                result.Append(list[0]);
        }

        return result.ToString();
    }

    ///<summary>
    ///	Cast this list into a List
    ///</summary>
    ///<param name = "source"></param>
    ///<typeparam name = "T"></typeparam>
    ///<returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static List<T> Cast<T>(this IList source)
    {
        var list = new List<T>();
        list.AddRange(source.OfType<T>());
        return list;
    }

    #region Merge

    /// <summary>The merge.</summary>
    /// <param name="lists">The lists.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static List<T> Merge<T>(params List<T>[] lists)
    {
        var merged = new List<T>();
        foreach (var list in lists) merged.Merge(list);
        return merged;
    }

    /// <summary>The merge.</summary>
    /// <param name="match">The match.</param>
    /// <param name="lists">The lists.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static List<T> Merge<T>(Expression<Func<T, object>> match, params List<T>[] lists)
    {
        var merged = new List<T>();
        foreach (var list in lists) merged.Merge(list, match);
        return merged;
    }

    /// <summary>The merge.</summary>
    /// <param name="list1">The list 1.</param>
    /// <param name="list2">The list 2.</param>
    /// <param name="match">The match.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static List<T> Merge<T>(this List<T> list1, List<T> list2, Expression<Func<T, object>> match)
    {
        if (list1 != null && list2 != null && match != null)
        {
            var matchFunc = match.Compile();
            foreach (var item in list2)
            {
                var key = matchFunc(item);
                if (!list1.Exists(i => matchFunc(i).Equals(key))) list1.Add(item);
            }
        }

        return list1;
    }

    /// <summary>The merge.</summary>
    /// <param name="list1">The list 1.</param>
    /// <param name="list2">The list 2.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <remarks>
    /// 	Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static List<T> Merge<T>(this List<T> list1, List<T> list2)
    {
        if (list1 != null && list2 != null) foreach (var item in list2.Where(item => !list1.Contains(item))) list1.Add(item);
        return list1;
    }

    #endregion
}
