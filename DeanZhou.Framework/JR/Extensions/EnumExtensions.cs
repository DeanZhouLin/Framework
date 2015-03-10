using System;
using System.Collections.Generic;
using System.ComponentModel;

/// <summary>
/// 枚举扩展类
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the textual description of the enum if it has one. e.g.
    /// 
    /// <code>
    /// enum UserColors
    /// {
    ///     [Description("Bright Red")]
    ///     BrightRed
    /// }
    /// UserColors.BrightRed.ToDescription();
    /// </code>
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static string ToDescription(this Enum @enum)
    {
        var type = @enum.GetType();
        var memInfo = type.GetMember(@enum.ToString());
        if (memInfo != null && memInfo.Length > 0)
        {
            var attrs = memInfo[0].GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attrs != null && attrs.Length > 0)
                return ((DescriptionAttribute)attrs[0]).Description;
        }

        return @enum.ToString();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="enum"></param>
    /// <returns></returns>
    public static List<string> ToList(this Enum @enum)
    {
        return new List<string>(Enum.GetNames(@enum.GetType()));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Has<T>(this Enum type, T value)
    {
        try
        {
            return (((int)(object)type & (int)(object)value) == (int)(object)value);
        }
        catch
        {
            return false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool Is<T>(this Enum type, T value)
    {
        try
        {
            return (int)(object)type == (int)(object)value;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T Add<T>(this Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type | (int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not append value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="type"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T Remove<T>(this Enum type, T value)
    {
        try
        {
            return (T)(object)(((int)(object)type & ~(int)(object)value));
        }
        catch (Exception ex)
        {
            throw new ArgumentException(
                string.Format(
                    "Could not remove value from enumerated type '{0}'.",
                    typeof(T).Name
                    ), ex);
        }
    }

}

