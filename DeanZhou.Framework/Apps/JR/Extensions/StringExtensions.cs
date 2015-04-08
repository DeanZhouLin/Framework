using System;
using System.Text;
/// <summary>
/// 	Extension methods for the string data type
/// </summary>
public static class StringExtensions
{
    #region Common string extensions

    /// <summary>
    /// 	Determines whether the specified string is null or empty.
    /// </summary>
    /// <param name = "value">The string value to check.</param>
    public static bool IsEmpty(this string value)
    {
        return string.IsNullOrEmpty(value);
    }

    /// <summary>
    /// 	Checks whether the string is empty and returns a default value in case.
    /// </summary>
    /// <param name = "value">The string to check.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <returns>Either the string or the default value.</returns>
    public static string IfEmpty(this string value, string defaultValue)
    {
        return (value.IsEmpty() ? defaultValue : value);
    }

    /// <summary>
    /// 	Formats the value with the parameters using string.Format.
    /// </summary>
    /// <param name = "value">The input string.</param>
    /// <param name = "parameters">The parameters.</param>
    /// <returns></returns>
    public static string FormatWith(this string value, params object[] parameters)
    {
        return string.Format(value, parameters);
    }

    /// <summary>
    /// 	Trims the text to a provided maximum length.
    /// </summary>
    /// <param name = "value">The input string.</param>
    /// <param name = "maxLength">Maximum length.</param>
    /// <returns></returns>
    /// <remarks>
    /// 	Proposed by Rene Schulte
    /// </remarks>
    public static string TrimToMaxLength(this string value, int maxLength)
    {
        return (value == null || value.Length <= maxLength ? value : value.Substring(0, maxLength));
    }

    /// <summary>
    /// 	Trims the text to a provided maximum length and adds a suffix if required.
    /// <para>例如：</para>
    /// <para>"十几年来，方兴东与马云每年一次，老友聚首，开怀畅谈，".TrimToMaxLength(10, "……")</para>
    /// </summary>
    /// <param name = "value">The input string.</param>
    /// <param name = "maxLength">Maximum length.</param>
    /// <param name = "suffix">The suffix.</param>
    /// <returns></returns>
    /// <remarks>
    /// 	Proposed by Rene Schulte
    /// </remarks>
    public static string TrimToMaxLength(this string value, int maxLength, string suffix)
    {
        return (value == null || value.Length <= maxLength ? value : string.Concat(value.Substring(0, maxLength), suffix));
    }

    /// <summary>
    /// 	Determines whether the comparison value strig is contained within the input value string
    /// </summary>
    /// <param name = "inputValue">The input value.</param>
    /// <param name = "comparisonValue">The comparison value.</param>
    /// <param name = "comparisonType">Type of the comparison to allow case sensitive or insensitive comparison.</param>
    /// <returns>
    /// 	<c>true</c> if input value contains the specified value, otherwise, <c>false</c>.
    /// </returns>
    public static bool Contains(this string inputValue, string comparisonValue, StringComparison comparisonType)
    {
        return (inputValue.IndexOf(comparisonValue, comparisonType) >= 0);
    }

    /// <summary>
    /// 	Repeats the specified string value as provided by the repeat count.
    /// </summary>
    /// <param name = "value">The original string.</param>
    /// <param name = "repeatCount">The repeat count.</param>
    /// <returns>The repeated string</returns>
    public static string Repeat(this string value, int repeatCount)
    {
        var sb = new StringBuilder();
        repeatCount.Times(() => sb.Append(value));
        return sb.ToString();
    }

    /// <summary>
    /// 	Concatenates the specified string value with the passed additional strings.
    /// </summary>
    /// <param name = "value">The original value.</param>
    /// <param name = "values">The additional string values to be concatenated.</param>
    /// <returns>The concatenated string.</returns>
    public static string ConcatWith(this string value, params string[] values)
    {
        return string.Concat(value, string.Concat(values));
    }
    #endregion
}
