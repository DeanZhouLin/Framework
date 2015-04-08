using System;
using System.Reflection;

namespace DeanZhou.Framework
{
    public static class Common
    {

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItemType"></typeparam>
        /// <typeparam name="TParamType"></typeparam>
        /// <typeparam name="TEnumType"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="fullClassName"></param>
        /// <returns></returns>
        public static IItemTypeIdentifier<TItemType, TParamType, TEnumType> CreateIdentifier<TItemType, TParamType, TEnumType>(string assemblyName, string fullClassName)
            where TItemType : class
            where TParamType : class
        {
            var ect = CreateInstance(assemblyName, fullClassName) as IItemTypeIdentifier<TItemType, TParamType, TEnumType>;
            return ect;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TItemType"></typeparam>
        /// <typeparam name="TParamType"></typeparam>
        /// <param name="assemblyName"></param>
        /// <param name="fullClassName"></param>
        /// <returns></returns>
        public static IFilter<TItemType, TParamType> CreateIFilter<TItemType, TParamType>(string assemblyName, string fullClassName)
            where TItemType : class
            where TParamType : class
        {
            var ect = CreateInstance(assemblyName, fullClassName) as IFilter<TItemType, TParamType>;
            return ect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="fullClassName"></param>
        /// <returns></returns>
        public static object CreateInstance(string assemblyName, string fullClassName)
        {
            return Assembly.Load(assemblyName).CreateInstance(fullClassName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumStr"></param>
        /// <returns></returns>
        public static T CreateEnum<T>(string enumStr)
        {
            return (T)Enum.Parse(typeof(T), enumStr, true);
        }

    }
}
