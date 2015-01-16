using System;
using System.Collections.Generic;
using System.Linq;

namespace DeanZhou.Framework
{
    [Flags]
    public enum NeedGetType
    {
        LongLen = 1,
        ShortLen = 2
    }

    public class CustomerFilterCore : ComplexFilterCore<string, string, NeedGetType>
    {
        public CustomerFilterCore(int currEachGetCount = 1, int maxCheckCount = 10) :
            base(currEachGetCount, maxCheckCount)
        {
            RegistItemTypeAsEnumType((item, param) =>
            {
                if (item.Length > 5)
                {
                    return NeedGetType.LongLen;
                }
                return NeedGetType.ShortLen;
            });

            RegistCustomerFilter((item, param) =>
            {
                int t;
                return int.TryParse(item, out t);
            });
        }
    }

    public interface IFilter<in TItemType, in TParamType>
        where TItemType : class
        where TParamType : class
    {
        bool DoFilter(TItemType itemType, TParamType paramType);
    }

    public interface IIdentifyItemTypeAsEnumType<in TItemType, in TParamType, out TEnumType>
        where TItemType : class
        where TParamType : class
    {
        TEnumType IdentifyItemTypeAsEnumType(TItemType itemType, TParamType paramType);
    }

    public sealed class SimpleFilterCore<TItemType, TParamType>
        where TItemType : class
        where TParamType : class
    {
        //自定义过滤器
        /// <summary>
        /// 自定义过滤器
        /// </summary>
        private Func<TItemType, TParamType, bool> CustomerFilter { get; set; }

        //检测当前政策行是否需要添加
        /// <summary>
        /// 检测当前政策行是否需要添加
        /// </summary>
        /// <param name="it"></param>
        /// <param name="pt"></param>
        public bool CheckCurrData(TItemType it, TParamType pt)
        {
            return CustomerFilter == null || CustomerFilter.GetInvocationList().All(x => x.DynamicInvoke(it, pt).ChangeType<bool>());
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void AddCustomerFilter(params Func<TItemType, TParamType, bool>[] customerFilters)
        {
            if (customerFilters == null)
            {
                CustomerFilter = null;
                return;
            }
            foreach (Func<TItemType, TParamType, bool> customerFilter in customerFilters)
            {
                if (CustomerFilter == null)
                {
                    CustomerFilter = customerFilter;
                }
                else
                {
                    CustomerFilter += customerFilter;
                }
            }
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void AddCustomerFilter(params IFilter<TItemType, TParamType>[] customerFilters)
        {
            if (customerFilters == null)
            {
                return;
            }
            foreach (IFilter<TItemType, TParamType> customerFilter in customerFilters)
            {
                AddCustomerFilter(customerFilter.DoFilter);
            }
        }
    }

    public class ComplexFilterCore<TItemType, TParamType, TEnumType>
        where TItemType : class
        where TParamType : class
        where TEnumType : struct
    {
        /// <summary>
        /// 枚举
        /// </summary>
        public static readonly Array EnumTypes;

        /// <summary>
        /// 当前需要提取的类型
        /// </summary>
        protected TEnumType CurrNeedType;

        /// <summary>
        /// 当前检测的对象
        /// </summary>
        protected IEnumerable<TItemType> WaitingCheckData;

        /// <summary>
        /// 最多匹配的有效数据的条数
        /// </summary>
        public int MaxCheckCount { get; set; }

        /// <summary>
        /// 每种类型需要提取的最小条数
        /// </summary>
        protected readonly Dictionary<TEnumType, int> EachTypeMinGetCount;

        /// <summary>
        /// 简单过滤器示例
        /// </summary>
        protected readonly SimpleFilterCore<TItemType, TParamType> SimpleFilterCore;

        /// <summary>
        /// 类型转换器
        /// </summary>
        protected Func<TItemType, TParamType, TEnumType> IdentifyItemTypeAsEnumType;

        /// <summary>
        /// 构造函数
        /// </summary>
        static ComplexFilterCore()
        {
            EnumTypes = Enum.GetValues(typeof(TEnumType));
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public ComplexFilterCore(int currEachGetCount = 1, int maxCheckCount = 10)
        {

            SimpleFilterCore = new SimpleFilterCore<TItemType, TParamType>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<TEnumType, int>();

            SetEachGetCount(currEachGetCount);

            //初始化最多检测的对象数
            SetMaxCheckCount(maxCheckCount);
        }

        /// <summary>
        /// 设置最大检查条数
        /// </summary>
        /// <param name="maxCheckCount"></param>
        public void SetMaxCheckCount(int maxCheckCount)
        {
            //初始化最多检测的对象数
            MaxCheckCount = maxCheckCount;
        }

        /// <summary>
        /// 设置每种类型需要获取的条数
        /// </summary>
        /// <param name="currEachGetCount"></param>
        public void SetEachGetCount(int currEachGetCount)
        {
            //初始化每种类型需要提取的个数
            foreach (TEnumType item in EnumTypes)
            {
                EachTypeMinGetCount[item] = currEachGetCount;
            }
        }

        /// <summary>
        /// 设置需要获取的类型
        /// </summary>
        /// <param name="currNeedType"></param>
        public void SetNeedType(TEnumType currNeedType)
        {
            //初始化需要类型     
            CurrNeedType = currNeedType;
        }

        /// <summary>
        /// 设置检查数据源
        /// </summary>
        /// <param name="waitingCheckData"></param>
        public void SetWaitingCheckData(IEnumerable<TItemType> waitingCheckData)
        {
            WaitingCheckData = waitingCheckData;
        }

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="identifyItemTypeAsEnumType"></param>
        protected void RegistItemTypeAsEnumType(Func<TItemType, TParamType, TEnumType> identifyItemTypeAsEnumType)
        {
            //初始化对象类型识别器
            IdentifyItemTypeAsEnumType = identifyItemTypeAsEnumType;
        }

        /// <summary>
        /// 注册自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        protected void RegistCustomerFilter(params Func<TItemType, TParamType, bool>[] customerFilters)
        {
            SimpleFilterCore.AddCustomerFilter(customerFilters);
        }

        /// <summary>
        /// 注册自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        protected void RegistCustomerFilter(params IFilter<TItemType, TParamType>[] customerFilters)
        {
            SimpleFilterCore.AddCustomerFilter(customerFilters);
        }

        /// <summary>
        /// 数据过滤
        /// </summary>
        /// <param name="pt"></param>
        /// <returns></returns>
        public virtual Dictionary<TItemType, TEnumType> FiltingData(TParamType pt)
        {
            Dictionary<TItemType, TEnumType> res = new Dictionary<TItemType, TEnumType>();

            //当前各个类型上已获取的个数
            Dictionary<TEnumType, int> currEachTypeGetedCount = new Dictionary<TEnumType, int>();
            foreach (TEnumType item in EnumTypes)
            {
                currEachTypeGetedCount[item] = 0;
            }

            int currCheckedCount = 0;

            foreach (TItemType itemType in WaitingCheckData)
            {
                //1 使用简单过滤器删选
                if (!SimpleFilterCore.CheckCurrData(itemType, pt))
                {
                    continue;
                }

                //2 通过过滤，识别当前对象的分类类型
                TEnumType et = IdentifyItemTypeAsEnumType(itemType, pt);

                //3 设置当前检测条数
                currCheckedCount++;

                //4 根据分类类型，判断该对象是否应该添加
                if (!IsValidate(et, currEachTypeGetedCount))
                {
                    continue;
                }

                //5 设置当前分类的获取数目
                SetEachGetCount(et, currEachTypeGetedCount);

                //6 添加结果
                res.Add(itemType, et);

                //7 是否已经完成
                if (res.Any() && IsFinished(currCheckedCount, currEachTypeGetedCount))
                {
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// 获取是否已经结束
        /// </summary>
        protected virtual bool IsFinished(int currCheckedCount, Dictionary<TEnumType, int> currEachTypeGetedCount)
        {
            //检测条数退出条件
            bool isNeedCountFinished = currCheckedCount >= MaxCheckCount;
            if (isNeedCountFinished)
            {
                return true;
            }

            //类型个数退出条件
            bool isFinished = currEachTypeGetedCount.All(dic =>
            {
                if ((dic.Key.ChangeType<int>() & CurrNeedType.ChangeType<int>()) == dic.Key.ChangeType<int>())
                {
                    return dic.Value >= EachTypeMinGetCount[dic.Key];
                }
                return true;
            });
            return isFinished;
        }

        /// <summary>
        /// 判断该记录是否应该要获取
        /// </summary>
        /// <param name="needPolicyType">需要获取的类型</param>
        /// <param name="currEachTypeGetedCount">已经获取的条数</param>
        /// <returns>true 需要 false 不需要</returns>
        protected virtual bool IsValidate(TEnumType needPolicyType, Dictionary<TEnumType, int> currEachTypeGetedCount)
        {
            return EnumTypes.Cast<TEnumType>().Any(enumType => ((enumType.ChangeType<int>() & needPolicyType.ChangeType<int>()) == enumType.ChangeType<int>()) && currEachTypeGetedCount[enumType] < EachTypeMinGetCount[enumType]);
        }

        /// <summary>
        /// 设置对应类型已经获取数
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="currEachTypeGetedCount"></param>
        private static void SetEachGetCount(TEnumType sourceType, Dictionary<TEnumType, int> currEachTypeGetedCount)
        {
            foreach (TEnumType targetType in EnumTypes.Cast<TEnumType>().Where(targetType => (sourceType.ChangeType<int>() & targetType.ChangeType<int>()) == targetType.ChangeType<int>()))
            {
                currEachTypeGetedCount[targetType]++;
            }
        }
    }
}
