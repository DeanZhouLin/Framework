using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 	Extension methods for the root data type object
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    /// 	Determines whether the object is equal to any of the provided values.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "obj">The object to be compared.</param>
    /// <param name = "values">The values to compare with the object.</param>
    /// <returns></returns>
    public static bool EqualsAny<T>(this T obj, params T[] values)
    {
        return (Array.IndexOf(values, obj) != -1);
    }

    /// <summary>
    /// 	Determines whether the object is equal to none of the provided values.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "obj">The object to be compared.</param>
    /// <param name = "values">The values to compare with the object.</param>
    /// <returns></returns>
    public static bool EqualsNone<T>(this T obj, params T[] values)
    {
        return (obj.EqualsAny(values) == false);
    }

    /// <summary>
    /// 	Converts an object to the specified target type or returns the default value.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <returns>The target type</returns>
    public static T ConvertTo<T>(this object value)
    {
        return value.ConvertTo(default(T));
    }

    /// <summary>
    /// 	Converts an object to the specified target type or returns the default value.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <returns>The target type</returns>
    public static T ConvertTo<T>(this object value, T defaultValue)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            if (value.GetType() == targetType) return (T)value;

            var converter = TypeDescriptor.GetConverter(value);
            if (converter != null)
            {
                if (converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(value, targetType);
            }

            converter = TypeDescriptor.GetConverter(targetType);
            if (converter != null)
            {
                if (converter.CanConvertFrom(value.GetType()))
                    return (T)converter.ConvertFrom(value);
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// 	Converts an object to the specified target type or returns the default value. Any exceptions are optionally ignored.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <param name = "ignoreException">if set to <c>true</c> ignore any exception.</param>
    /// <returns>The target type</returns>
    public static T ConvertTo<T>(this object value, T defaultValue, bool ignoreException)
    {
        if (ignoreException)
        {
            try
            {
                return value.ConvertTo<T>();
            }
            catch
            {
                return defaultValue;
            }
        }
        return value.ConvertTo<T>();
    }

    /// <summary>
    /// 	Determines whether the value can (in theory) be converted to the specified target type.
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <returns>
    /// 	<c>true</c> if this instance can be convert to the specified target type; otherwise, <c>false</c>.
    /// </returns>
    public static bool CanConvertTo<T>(this object value)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            var converter = TypeDescriptor.GetConverter(value);
            if (converter != null)
            {
                if (converter.CanConvertTo(targetType))
                    return true;
            }

            converter = TypeDescriptor.GetConverter(targetType);
            if (converter != null)
            {
                if (converter.CanConvertFrom(value.GetType()))
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 	Cast an object to the given type. Usefull especially for anonymous types.
    /// </summary>
    /// <typeparam name = "T">The type to cast to</typeparam>
    /// <param name = "value">The object to case</param>
    /// <returns>
    /// 	the casted type or null if casting is not possible.
    /// </returns>
    /// <remarks>
    /// 	Contributed by blaumeister, http://www.codeplex.com/site/users/view/blaumeiser
    /// </remarks>
    public static T CastTo<T>(this object value)
    {
        return (T)value;
    }

    /// <summary>
    /// 	Returns TRUE, if specified target reference is equals with null reference.
    /// 	Othervise returns FALSE.
    /// </summary>
    /// <param name = "target">Target reference. Can be null.</param>
    /// <remarks>
    /// 	Some types has overloaded '==' and '!=' operators.
    /// 	So the code "null == ((MyClass)null)" can returns <c>false</c>.
    /// 	The most correct way how to test for null reference is using "System.Object.ReferenceEquals(object, object)" method.
    /// 	However the notation with ReferenceEquals method is long and uncomfortable - this extension method solve it.
    /// 
    /// 	Contributed by tencokacistromy, http://www.codeplex.com/site/users/view/tencokacistromy
    /// </remarks>
    /// <example>
    /// 	object someObject = GetSomeObject();
    /// 	if ( someObject.IsNull() ) { /* the someObject is null */ }
    /// 	else { /* the someObject is not null */ }
    /// </example>
    public static bool IsNull(this object target)
    {
        var ret = IsNull<object>(target);
        return ret;
    }

    /// <summary>
    /// 	Returns TRUE, if specified target reference is equals with null reference.
    /// 	Othervise returns FALSE.
    /// </summary>
    /// <typeparam name = "T">Type of target.</typeparam>
    /// <param name = "target">Target reference. Can be null.</param>
    /// <remarks>
    /// 	Some types has overloaded '==' and '!=' operators.
    /// 	So the code "null == ((MyClass)null)" can returns <c>false</c>.
    /// 	The most correct way how to test for null reference is using "System.Object.ReferenceEquals(object, object)" method.
    /// 	However the notation with ReferenceEquals method is long and uncomfortable - this extension method solve it.
    /// 
    /// 	Contributed by tencokacistromy, http://www.codeplex.com/site/users/view/tencokacistromy
    /// </remarks>
    /// <example>
    /// 	MyClass someObject = GetSomeObject();
    /// 	if ( someObject.IsNull() ) { /* the someObject is null */ }
    /// 	else { /* the someObject is not null */ }
    /// </example>
    public static bool IsNull<T>(this T target)
    {
        return ReferenceEquals(target, null);        
    }
    /// <summary>
    /// 如果当前对象是Null则返回defaultValue
    /// </summary>
    /// <typeparam name="T">参数类型</typeparam>
    /// <param name="target">当前参数</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>如果当前对象是Null则返回defaultValue，否则返回当前对象</returns>
    public static T IfNull<T>(this T target, T defaultValue)
    {
        return target.IsNull() ? defaultValue : target;
    }
    /// <summary>
    /// Cast an object to the given type. Usefull especially for anonymous types.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object to be cast</param>
    /// <returns>
    /// the casted type or null if casting is not possible.
    /// </returns>
    /// <remarks>
    /// Contributed by Michael T, http://about.me/MichaelTran
    /// </remarks>
    public static T CastAs<T>(this object obj) where T : class, new()
    {
        return obj as T;
    }

    /// <summary>
    /// 深度拷贝一个对象，拷贝的类型必须是可序列化的
    /// </summary>
    /// <typeparam name="T">The type of object being copied.</typeparam>
    /// <param name="source">The object instance to copy.</param>
    /// <returns>The copied object.</returns>
    public static T Clone<T>(this T source)
    {
        if (!typeof(T).IsSerializable)
        {
            throw new ArgumentException("The type must be serializable.", "source");
        }

        // Don't serialize a null object, simply return the default for that object
        if (Object.ReferenceEquals(source, null))
        {
            return default(T);
        }

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new MemoryStream();
        using (stream)
        {
            formatter.Serialize(stream, source);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)formatter.Deserialize(stream);
        }
    }
}
