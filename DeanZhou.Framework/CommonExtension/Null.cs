using System;

namespace DeanZhou.Framework
{
    public class Null
    {
        public static short NullShort
        {
            get
            {
                return 0;
            }
        }

        public static int NullInteger
        {
            get
            {
                return 0;
            }
        }

        public static int NullLong
        {
            get
            {
                return 0;
            }
        }

        public static byte NullByte
        {
            get
            {
                return 0;
            }
        }

        public static float NullSingle
        {
            get
            {
                return 0f;
            }
        }

        public static double NullDouble
        {
            get
            {
                return 0d;
            }
        }

        public static decimal NullDecimal
        {
            get
            {
                return 0m;
            }
        }

        public static DateTime NullDate
        {
            get
            {
                return DateTime.MinValue;
            }
        }

        public static string NullString
        {
            get
            {
                return "";
            }
        }

        public static bool NullBoolean
        {
            get
            {
                return false;
            }
        }

        public static Guid NullGuid
        {
            get
            {
                return Guid.Empty;
            }
        }

        /// <summary>
        /// 获取某数值类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetNull(Type type)
        {
            return SetNull(DBNull.Value, type);
        }

        /// <summary>
        /// 如果给定objValue为空，则赋该类型的默认值
        /// </summary>
        /// <param name="objValue"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object SetNull(object objValue, Type type)
        {
            object returnValue;
            if (objValue == DBNull.Value)
            {
                if (type == typeof(short))
                {
                    returnValue = NullShort;
                }
                else if (type == typeof(byte))
                {
                    returnValue = NullByte;
                }
                else if (type == typeof(int))
                {
                    returnValue = NullInteger;
                }
                else if (type == typeof(long))
                {
                    returnValue = NullLong;
                }
                else if (type == typeof(float))
                {
                    returnValue = NullSingle;
                }
                else if (type == typeof(double))
                {
                    returnValue = NullDouble;
                }
                else if (type == typeof(decimal))
                {
                    returnValue = NullDecimal;
                }
                else if (type == typeof(DateTime))
                {
                    returnValue = NullDate;
                }
                else if (type == typeof(string))
                {
                    returnValue = NullString;
                }
                else if (type == typeof(bool))
                {
                    returnValue = NullBoolean;
                }
                else if (type == typeof(Guid))
                {
                    returnValue = NullGuid;
                }
                else //complex object
                {
                    returnValue = null;
                }
            }
            else //return value
            {
                returnValue = objValue;
            }
            return returnValue;
        }

        /// <summary>
        /// 判断值objField，是否为空值
        /// </summary>
        /// <param name="objField"></param>
        /// <returns></returns>
        public static bool IsNull(object objField)
        {
            bool isNull;
            if (objField != null)
            {
                if (objField is int)
                {
                    isNull = objField.Equals(NullInteger);
                }
                else if (objField is short)
                {
                    isNull = objField.Equals(NullShort);
                }
                else if (objField is long)
                {
                    isNull = objField.Equals(NullLong);
                }
                else if (objField is byte)
                {
                    isNull = objField.Equals(NullByte);
                }
                else if (objField is float)
                {
                    isNull = objField.Equals(NullSingle);
                }
                else if (objField is double)
                {
                    isNull = objField.Equals(NullDouble);
                }
                else if (objField is decimal)
                {
                    isNull = objField.Equals(NullDecimal);
                }
                else if (objField is DateTime)
                {
                    DateTime objDate = (DateTime)objField;
                    isNull = objDate.Date.Equals(NullDate.Date);
                }
                else if (objField is string)
                {
                    isNull = objField.Equals(NullString);
                }
                else if (objField is bool)
                {
                    isNull = objField.Equals(NullBoolean);
                }
                else if (objField is Guid)
                {
                    isNull = objField.Equals(NullGuid);
                }
                else //complex object
                {
                    isNull = false;
                }
            }
            else
            {
                isNull = true;
            }
            return isNull;
        }
    }
}
