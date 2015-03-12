using System;
using System.Collections.Generic;
using System.Linq;

namespace DeanZhou.Framework
{
    /// <summary>
    /// 复杂过滤器
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    /// <typeparam name="TEnumType"></typeparam>
    public class ComplexFilterCore<TItemType, TEnumType>
        where TItemType : class
        where TEnumType : struct
    {

        #region 构造函数

        /// <summary>
        /// 枚举
        /// </summary>
        public static readonly IEnumerable<TEnumType> EnumTypes;

        /// <summary>
        /// 构造函数
        /// </summary>
        static ComplexFilterCore()
        {
            EnumTypes = Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComplexFilterCore()
        {
            //简单过滤器实例 过滤单条数据
            SimpleFilterCore = new SimpleFilterCore<TItemType>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<TEnumType, int>();
        }

        #endregion

        #region SetMinGetCount

        /// <summary>
        /// 当前需要提取的类型 
        /// </summary>
        protected int CurrNeedType;

        /// <summary>
        /// 退出阀值
        /// </summary>
        protected int QuitValue;

        /// <summary>
        /// 每种类型需要提取的最小条数 *
        /// </summary>
        protected readonly Dictionary<TEnumType, int> EachTypeMinGetCount;

        /// <summary>
        /// 设置对应类型需要获取的最大条数
        /// </summary>
        public ComplexFilterCore<TItemType, TEnumType> SetMinGetCount(TEnumType needEnumType, int needGetCount)
        {
            if (EachTypeMinGetCount.Count == 0)
            {
                foreach (TEnumType item in EnumTypes)
                {
                    EachTypeMinGetCount.Add(item, 0);
                }
            }

            IEnumerable<TEnumType> validEnums = EnumTypes.Where(tt => (needEnumType.ChangeType<int>() & tt.ChangeType<int>()) == tt.ChangeType<int>()).ToList();

            foreach (TEnumType validEnumType in validEnums)
            {
                EachTypeMinGetCount[validEnumType] = needGetCount;
            }

            CurrNeedType = EachTypeMinGetCount.First(c => c.Value > 0).Key.ChangeType<int>();

            bool isFirst = true;
            foreach (KeyValuePair<TEnumType, int> kv in EachTypeMinGetCount)
            {
                if (isFirst)
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = kv.Key.ChangeType<int>();
                        isFirst = false;
                    }
                }
                else
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = (CurrNeedType.ChangeType<int>() | kv.Key.ChangeType<int>());
                    }
                }
            }

            QuitValue = EachTypeMinGetCount.Values.Sum();
            return this;
        }

        /// <summary>
        /// 设置对应类型需要获取的最大条数
        /// </summary>
        /// <param name="needEnumTypeName"></param>
        /// <param name="needGetCount"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TEnumType> SetMinGetCount(string needEnumTypeName, int needGetCount)
        {
            if (!string.IsNullOrEmpty(needEnumTypeName))
            {
                foreach (string str in needEnumTypeName.Split('|').Where(c => !string.IsNullOrEmpty(c)))
                {
                    SetMinGetCount(Common.CreateEnum<TEnumType>(str), needGetCount);
                }

            }
            return this;
        }

        #endregion

        #region RegistEnumTypeIdentifier

        /// <summary>
        /// 类型转换器 *
        /// </summary>
        protected Func<TItemType, TEnumType> EnumTypeIdentifier;

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="enumTypeIdentifier">类型识别器</param>
        public ComplexFilterCore<TItemType, TEnumType> RegistEnumTypeIdentifier(Func<TItemType, TEnumType> enumTypeIdentifier)
        {
            //初始化对象类型识别器
            EnumTypeIdentifier = enumTypeIdentifier;
            return this;
        }

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="enumTypeIdentifier"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TEnumType> RegistEnumTypeIdentifier(IEnumTypeIdentifier<TItemType, TEnumType> enumTypeIdentifier)
        {
            if (enumTypeIdentifier == null)
            {
                return this;
            }
            //初始化对象类型识别器
            EnumTypeIdentifier = enumTypeIdentifier.IdentifyItemTypeAsEnumType;
            return this;
        }

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="identifierFullClassName"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TEnumType> RegistEnumTypeIdentifier(string assemblyName,
            string identifierFullClassName)
        {
            var instance = Common.CreateIdentifier<TItemType, TEnumType>(assemblyName, identifierFullClassName);
            return RegistEnumTypeIdentifier(instance);
        }

        #endregion

        #region AddFilter

        /// <summary>
        /// 简单过滤器实例 *
        /// </summary>
        protected readonly SimpleFilterCore<TItemType> SimpleFilterCore;

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public ComplexFilterCore<TItemType, TEnumType> AddFilter(params IFilter<TItemType>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TEnumType> AddFilter(params Func<TItemType, bool>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="filterFullClassNames"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TEnumType> AddFilter(string assemblyName, params string[] filterFullClassNames)
        {
            SimpleFilterCore.AddFilter(assemblyName, filterFullClassNames);
            return this;
        }

        #endregion

        #region GetFilteredResult

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessDataList"></param>
        /// <returns></returns>
        public virtual Dictionary<TItemType, TEnumType> GetFilteredResult(IEnumerable<TItemType> waitProcessDataList)
        {
            Dictionary<TItemType, TEnumType> res = new Dictionary<TItemType, TEnumType>();

            //当前各个类型上已获取的个数
            Dictionary<TEnumType, int> currEachTypeGetedCount = new Dictionary<TEnumType, int>();
            foreach (TEnumType item in EnumTypes)
            {
                currEachTypeGetedCount[item] = 0;
            }

            int currCheckedCount = 0;

            foreach (TItemType itemType in waitProcessDataList)
            {
                //1 使用简单过滤器删选
                if (!SimpleFilterCore.CheckCurrData(itemType))
                {
                    continue;
                }

                //2 通过过滤，识别当前对象的分类类型
                if (EnumTypeIdentifier == null)
                {
                    throw new Exception("未注册类型识别器");
                }
                TEnumType et = EnumTypeIdentifier(itemType);
                IEnumerable<TEnumType> validEnums = EnumTypes.Where(targetType => (et.ChangeType<int>() & targetType.ChangeType<int>()) == targetType.ChangeType<int>()).ToList();

                //3 设置当前检测条数
                currCheckedCount++;

                //4 根据分类类型，判断该对象是否应该添加
                if (EachTypeMinGetCount.All(c => c.Value == 0) || validEnums.Any(enumType => currEachTypeGetedCount[enumType] < EachTypeMinGetCount[enumType]))
                {
                    //5 更新当前分类的获取数目
                    foreach (TEnumType targetType in validEnums)
                    {
                        currEachTypeGetedCount[targetType]++;
                    }

                    //6 添加结果
                    res.Add(itemType, et);
                }

                //7 是否已经完成
                if (IsFinished(currCheckedCount, currEachTypeGetedCount))
                {
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// 判断本次过滤是否完成
        /// </summary>
        protected virtual bool IsFinished(int currCheckedCount, Dictionary<TEnumType, int> currEachTypeGetedCount)
        {
            //检测条数退出条件
            bool isNeedCountFinished = QuitValue > 0 && currCheckedCount >= QuitValue;
            if (isNeedCountFinished)
            {
                return true;
            }

            //类型个数退出条件
            bool isFinished = currEachTypeGetedCount.All(dic =>
            {
                if ((dic.Key.ChangeType<int>() & CurrNeedType) == dic.Key.ChangeType<int>())
                {
                    return dic.Value >= EachTypeMinGetCount[dic.Key];
                }
                return true;
            });
            return isFinished;
        }

        #endregion

    }

    /// <summary>
    /// 复杂过滤器
    /// </summary>
    /// <typeparam name="TItemType"></typeparam>
    /// <typeparam name="TParamType"></typeparam>
    /// <typeparam name="TEnumType"></typeparam>
    public class ComplexFilterCore<TItemType, TParamType, TEnumType>
        where TItemType : class
        where TParamType : class
        where TEnumType : struct
    {

        #region 构造函数

        /// <summary>
        /// 枚举
        /// </summary>
        public static readonly IEnumerable<TEnumType> EnumTypes;

        /// <summary>
        /// 构造函数
        /// </summary>
        static ComplexFilterCore()
        {
            EnumTypes = Enum.GetValues(typeof(TEnumType)).Cast<TEnumType>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComplexFilterCore()
        {
            //简单过滤器实例 过滤单条数据
            SimpleFilterCore = new SimpleFilterCore<TItemType, TParamType>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<TEnumType, int>();
        }

        #endregion

        #region SetMinGetCount

        /// <summary>
        /// 当前需要提取的类型 
        /// </summary>
        protected int CurrNeedType;

        /// <summary>
        /// 退出阀值
        /// </summary>
        protected int QuitValue;

        /// <summary>
        /// 每种类型需要提取的最小条数 *
        /// </summary>
        protected readonly Dictionary<TEnumType, int> EachTypeMinGetCount;

        /// <summary>
        /// 设置对应类型需要获取的最大条数
        /// </summary>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> SetMinGetCount(TEnumType needEnumType, int needGetCount)
        {
            if (EachTypeMinGetCount.Count == 0)
            {
                foreach (TEnumType item in EnumTypes)
                {
                    EachTypeMinGetCount.Add(item, 0);
                }
            }

            IEnumerable<TEnumType> validEnums = EnumTypes.Where(tt => (needEnumType.ChangeType<int>() & tt.ChangeType<int>()) == tt.ChangeType<int>()).ToList();

            foreach (TEnumType validEnumType in validEnums)
            {
                EachTypeMinGetCount[validEnumType] = needGetCount;
            }

            CurrNeedType = EachTypeMinGetCount.First(c => c.Value > 0).Key.ChangeType<int>();

            bool isFirst = true;
            foreach (KeyValuePair<TEnumType, int> kv in EachTypeMinGetCount)
            {
                if (isFirst)
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = kv.Key.ChangeType<int>();
                        isFirst = false;
                    }
                }
                else
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = (CurrNeedType.ChangeType<int>() | kv.Key.ChangeType<int>());
                    }
                }
            }

            QuitValue = EachTypeMinGetCount.Values.Sum();
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="needEnumTypeName"></param>
        /// <param name="needGetCount"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> SetMinGetCount(string needEnumTypeName,
            int needGetCount)
        {
            if (!string.IsNullOrEmpty(needEnumTypeName))
            {
                foreach (string str in needEnumTypeName.Split('|').Where(c => !string.IsNullOrEmpty(c)))
                {
                    SetMinGetCount(Common.CreateEnum<TEnumType>(str), needGetCount);
                }
            }
            return this;
        }

        #endregion

        #region RegistEnumTypeIdentifier

        /// <summary>
        /// 类型转换器 *
        /// </summary>
        protected Func<TItemType, TParamType, TEnumType> EnumTypeIdentifier;

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="enumTypeIdentifier">类型识别器</param>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> RegistEnumTypeIdentifier(Func<TItemType, TParamType, TEnumType> enumTypeIdentifier)
        {
            //初始化对象类型识别器
            EnumTypeIdentifier = enumTypeIdentifier;
            return this;
        }

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="enumTypeIdentifier"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> RegistEnumTypeIdentifier(IEnumTypeIdentifier<TItemType, TParamType, TEnumType> enumTypeIdentifier)
        {
            if (enumTypeIdentifier == null)
            {
                return this;
            }
            //初始化对象类型识别器
            EnumTypeIdentifier = enumTypeIdentifier.IdentifyItemTypeAsEnumType;
            return this;
        }

        #endregion

        #region AddFilter

        /// <summary>
        /// 简单过滤器实例 *
        /// </summary>
        protected readonly SimpleFilterCore<TItemType, TParamType> SimpleFilterCore;

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> AddFilter(params IFilter<TItemType, TParamType>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public ComplexFilterCore<TItemType, TParamType, TEnumType> AddFilter(params Func<TItemType, TParamType, bool>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        public ComplexFilterCore<TItemType, TParamType, TEnumType> AddFilter(string assemblyName, params string[] filterFullClassNames)
        {
            SimpleFilterCore.AddFilter(assemblyName, filterFullClassNames);
            return this;
        }
        #endregion

        #region GetFilteredResult

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessDataList"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public virtual Dictionary<TItemType, TEnumType> GetFilteredResult(IEnumerable<TItemType> waitProcessDataList, TParamType pt)
        {
            Dictionary<TItemType, TEnumType> res = new Dictionary<TItemType, TEnumType>();

            //当前各个类型上已获取的个数
            Dictionary<TEnumType, int> currEachTypeGetedCount = new Dictionary<TEnumType, int>();
            foreach (TEnumType item in EnumTypes)
            {
                currEachTypeGetedCount[item] = 0;
            }

            int currCheckedCount = 0;

            foreach (TItemType itemType in waitProcessDataList)
            {
                //1 使用简单过滤器删选
                if (!SimpleFilterCore.CheckCurrData(itemType, pt))
                {
                    continue;
                }

                //2 通过过滤，识别当前对象的分类类型
                if (EnumTypeIdentifier == null)
                {
                    throw new Exception("未注册类型识别器");
                }
                TEnumType et = EnumTypeIdentifier(itemType, pt);
                IEnumerable<TEnumType> validEnums = EnumTypes.
                    Where(targetType => (et.ChangeType<int>() & targetType.ChangeType<int>())
                        == targetType.ChangeType<int>()).ToList();

                //3 设置当前检测条数
                currCheckedCount++;

                //4 根据分类类型，判断该对象是否应该添加
                if (EachTypeMinGetCount.All(c => c.Value == 0) ||
                    validEnums.Any(enumType => currEachTypeGetedCount[enumType] < EachTypeMinGetCount[enumType]))
                {
                    //5 更新当前分类的获取数目
                    foreach (TEnumType targetType in validEnums)
                    {
                        currEachTypeGetedCount[targetType]++;
                    }

                    //6 添加结果
                    res.Add(itemType, et);
                }

                //7 是否已经完成
                if (IsFinished(currCheckedCount, currEachTypeGetedCount))
                {
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// 判断本次过滤是否完成
        /// </summary>
        protected virtual bool IsFinished(int currCheckedCount, Dictionary<TEnumType, int> currEachTypeGetedCount)
        {
            //检测条数退出条件
            bool isNeedCountFinished = QuitValue > 0 && currCheckedCount >= QuitValue;
            if (isNeedCountFinished)
            {
                return true;
            }

            //类型个数退出条件
            bool isFinished = currEachTypeGetedCount.All(dic =>
            {
                if ((dic.Key.ChangeType<int>() & CurrNeedType) == dic.Key.ChangeType<int>())
                {
                    return dic.Value >= EachTypeMinGetCount[dic.Key];
                }
                return true;
            });
            return isFinished;
        }

        #endregion

    }
}
