using System;
using System.Collections.Generic;
using System.Linq;

namespace DeanZhou.Framework
{

    /// <summary>
    /// 复杂过滤器
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TParam"></typeparam>
    /// <typeparam name="TItemType"></typeparam>
    public class ListFilterCore<TItem, TParam, TItemType>
        where TItem : class
        where TParam : class
        where TItemType : struct
    {
        #region 构造函数

        /// <summary>
        /// 枚举
        /// </summary>
        public static readonly IEnumerable<TItemType> EnumTypes;

        /// <summary>
        /// 构造函数
        /// </summary>
        static ListFilterCore()
        {
            EnumTypes = Enum.GetValues(typeof(TItemType)).Cast<TItemType>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ListFilterCore()
        {
            //简单过滤器实例 过滤单条数据
            SimpleFilterCore = new ElementFilterCore<TItem, TParam>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<TItemType, int>();
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
        protected readonly Dictionary<TItemType, int> EachTypeMinGetCount;

        /// <summary>
        /// 设置对应类型需要获取的最大条数
        /// </summary>
        public ListFilterCore<TItem, TParam, TItemType> SetMinGetCount(TItemType needEnumType, int needGetCount)
        {
            if (EachTypeMinGetCount.Count == 0)
            {
                foreach (TItemType item in EnumTypes)
                {
                    EachTypeMinGetCount.Add(item, 0);
                }
            }

            IEnumerable<TItemType> validEnums = EnumTypes.Where(tt => needEnumType.HasItem(tt)).ToList();

            foreach (TItemType validEnumType in validEnums)
            {
                EachTypeMinGetCount[validEnumType] = needGetCount;
            }

            CurrNeedType = EachTypeMinGetCount.First(c => c.Value > 0).Key.ChangeType<int>();

            bool isFirst = true;
            foreach (KeyValuePair<TItemType, int> kv in EachTypeMinGetCount)
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
        public ListFilterCore<TItem, TParam, TItemType> SetMinGetCount(string needEnumTypeName,
            int needGetCount)
        {
            if (!string.IsNullOrEmpty(needEnumTypeName))
            {
                foreach (string str in needEnumTypeName.Split('|').Where(c => !string.IsNullOrEmpty(c)))
                {
                    SetMinGetCount(Common.CreateEnum<TItemType>(str), needGetCount);
                }
            }
            return this;
        }

        #endregion

        #region RegistEnumTypeIdentifier

        /// <summary>
        /// 类型转换器 *
        /// </summary>
        protected Func<TItem, TParam, TItemType> EnumTypeIdentifier;

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="enumTypeIdentifier">类型识别器</param>
        public ListFilterCore<TItem, TParam, TItemType> RegistEnumTypeIdentifier(Func<TItem, TParam, TItemType> enumTypeIdentifier)
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
        public ListFilterCore<TItem, TParam, TItemType> RegistEnumTypeIdentifier(IItemTypeIdentifier<TItem, TParam, TItemType> enumTypeIdentifier)
        {
            if (enumTypeIdentifier == null)
            {
                return this;
            }
            //初始化对象类型识别器
            EnumTypeIdentifier = enumTypeIdentifier.IdentifyItemType;
            return this;
        }

        #endregion

        #region AddFilter

        /// <summary>
        /// 简单过滤器实例 *
        /// </summary>
        protected readonly ElementFilterCore<TItem, TParam> SimpleFilterCore;

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public ListFilterCore<TItem, TParam, TItemType> AddFilter(params IFilter<TItem, TParam>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public ListFilterCore<TItem, TParam, TItemType> AddFilter(params Func<TItem, TParam, bool>[] filters)
        {
            SimpleFilterCore.AddFilter(filters);
            return this;
        }

        public ListFilterCore<TItem, TParam, TItemType> AddFilter(string assemblyName, params string[] filterFullClassNames)
        {
            SimpleFilterCore.AddFilter(assemblyName, filterFullClassNames);
            return this;
        }
        #endregion

        #region DoFilter

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessDataList"></param>
        /// <param name="pt"></param>
        /// <returns></returns>
        public virtual Dictionary<TItem, TItemType> DoFilter(IEnumerable<TItem> waitProcessDataList, TParam pt)
        {
            if (EnumTypeIdentifier == null)
            {
                throw new Exception("未注册类型识别器");
            }

            Dictionary<TItem, TItemType> res = new Dictionary<TItem, TItemType>();

            //当前各个类型上已获取的个数
            Dictionary<TItemType, int> currEachTypeGetedCount = new Dictionary<TItemType, int>();
            foreach (TItemType item in EnumTypes)
            {
                currEachTypeGetedCount[item] = 0;
            }

            int currCheckedCount = 0;

            foreach (TItem itemType in waitProcessDataList)
            {
                TItemType et = EnumTypeIdentifier(itemType, pt);
                IEnumerable<TItemType> validEnums = EnumTypes.Where(targetType => et.HasItem(targetType)).ToList();

                if (validEnums.All(enumType => currEachTypeGetedCount[enumType] >= EachTypeMinGetCount[enumType]))
                {
                    continue;
                }

                if (!SimpleFilterCore.DoFilter(itemType, pt))
                {
                    continue;
                }
                currCheckedCount++;

                foreach (TItemType targetType in validEnums)
                {
                    currEachTypeGetedCount[targetType]++;
                }

                res.Add(itemType, et);
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
        protected virtual bool IsFinished(int currCheckedCount, Dictionary<TItemType, int> currEachTypeGetedCount)
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
    /// <typeparam name="TItem">你需要过滤的对象的类型</typeparam>
    /// <typeparam name="TParam">过滤一个对象需要的辅助参数的类型</typeparam>
    public class ListFilterCore<TItem, TParam>
        where TItem : class
        where TParam : class
    {
        /// <summary>
        /// 简单过滤器实例 *
        /// </summary>
        protected readonly ElementFilterCore<TItem, TParam> ElementFilterCore;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ListFilterCore()
        {
            //简单过滤器实例 过滤单条数据
            ElementFilterCore = new ElementFilterCore<TItem, TParam>();
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public ListFilterCore<TItem, TParam> AddFilter(params IFilter<TItem, TParam>[] filters)
        {
            ElementFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        public ListFilterCore<TItem, TParam> AddFilter(params Func<TItem, TParam, bool>[] filters)
        {
            ElementFilterCore.AddFilter(filters);
            return this;
        }

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessItemList"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual List<TItem> DoFilter(IEnumerable<TItem> waitProcessItemList, TParam param)
        {
            if (waitProcessItemList == null)
            {
                return new List<TItem>();
            }

            List<TItem> res = new List<TItem>();

            foreach (TItem item in waitProcessItemList)
            {
                //使用简单过滤器删选
                if (!ElementFilterCore.DoFilter(item, param))
                {
                    continue;
                }

                //添加结果
                res.Add(item);
            }

            return res;
        }
    }
}
