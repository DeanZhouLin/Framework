using System;
using System.Collections.Generic;
using System.Linq;
using PostSharp.Patterns.Threading;

namespace DeanZhou.Framework
{
    public abstract class FilterBase<ItemType, ParamType>
        where ItemType : class
        where ParamType : class
    {
        public abstract bool DoFilter(ItemType itemType, ParamType paramType);
    }

    public class SimpleFilterCore<ItemType, ParamType>
        where ItemType : class
        where ParamType : class
    {
        //自定义过滤器
        /// <summary>
        /// 自定义过滤器
        /// </summary>
        private Func<ItemType, ParamType, bool> CustomerFilter { get; set; }

        //检测当前政策行是否需要添加
        /// <summary>
        /// 检测当前政策行是否需要添加
        /// </summary>
        /// <param name="it"></param>
        /// <param name="pt"></param>
        public bool CheckCurrData(ItemType it, ParamType pt)
        {
            return CustomerFilter == null || CustomerFilter.GetInvocationList().All(x => x.DynamicInvoke(it, pt).ChangeType<bool>());
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void AddCustomerFilter(params Func<ItemType, ParamType, bool>[] customerFilters)
        {
            if (customerFilters == null)
            {
                CustomerFilter = null;
                return;
            }
            foreach (Func<ItemType, ParamType, bool> customerFilter in customerFilters)
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
        public void AddCustomerFilter(params FilterBase<ItemType, ParamType>[] customerFilters)
        {
            if (customerFilters == null)
            {
                return;
            }
            foreach (FilterBase<ItemType, ParamType> customerFilter in customerFilters)
            {
                AddCustomerFilter(customerFilter.DoFilter);
            }
        }
    }

    public class ComplexFilterCore<ItemType, ParamType, EnumType>
        where ItemType : class
        where ParamType : class
        where EnumType : struct
    {
        // 当前需要提取的类型
        /// <summary>
        /// 当前需要提取的类型
        /// </summary>
        private EnumType CurrNeedType { get; set; }

        //当前已经匹配的有效数据的条数
        /// <summary>
        /// 当前已经匹配的有效数据的条数
        /// </summary>
        private int CurrCheckedCount { get; set; }

        //最多匹配的有效数据的条数
        /// <summary>
        /// 最多匹配的有效数据的条数
        /// </summary>
        private int MaxCheckCount { get; set; }

        //每种类型需要提取的最小条数
        /// <summary>
        /// 每种类型需要提取的最小条数
        /// </summary>
        private Dictionary<EnumType, int> EachTypeMinGetCount { get; set; }

        //每种类型已经提取的条数
        /// <summary>
        /// 每种类型已经提取的条数
        /// </summary>
        private Dictionary<EnumType, int> CurrEachTypeGetedCount { get; set; }

        //当前检测的对象
        /// <summary>
        /// 当前检测的对象
        /// </summary>
        protected IEnumerable<ItemType> CurrItems { get; private set; }

        //获取是否已经结束
        /// <summary>
        /// 获取是否已经结束
        /// </summary>
        public bool IsFinished
        {
            get
            {
                if (!CurrPickedData.Any())
                {
                    return false;
                }

                //检测条数退出条件
                bool isNeedCountFinished = CurrCheckedCount >= MaxCheckCount;
                if (isNeedCountFinished)
                {
                    return true;
                }

                //类型个数退出条件
                bool isFinished = CurrEachTypeGetedCount.All(dic =>
                {
                    if ((dic.Key.ChangeType<int>() & CurrNeedType.ChangeType<int>()) == dic.Key.ChangeType<int>())
                    {
                        return dic.Value >= EachTypeMinGetCount[dic.Key];
                    }
                    return true;
                });
                return isFinished;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly SimpleFilterCore<ItemType, ParamType> SimpleFilterCore;

        //类型转换器
        /// <summary>
        /// 类型转换器
        /// </summary>
        private Func<ItemType, ParamType, EnumType> IdentifyItemTypeAsEnumType { get; set; }

        //构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public ComplexFilterCore(IEnumerable<ItemType> waitCheckItems,
            Func<ItemType, ParamType, EnumType> identifyItemTypeAsEnumType,
            EnumType currNeedType,
            int currEachGetCount = 1,
            int maxCheckCount = 10)
        {
            CurrItems = waitCheckItems;
            SimpleFilterCore = new SimpleFilterCore<ItemType, ParamType>();
            Init(identifyItemTypeAsEnumType, currNeedType, currEachGetCount, maxCheckCount);
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void AddCustomerFilter(params Func<ItemType, ParamType, bool>[] customerFilters)
        {
            SimpleFilterCore.AddCustomerFilter(customerFilters);
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void AddCustomerFilter(params FilterBase<ItemType, ParamType>[] customerFilters)
        {
            SimpleFilterCore.AddCustomerFilter(customerFilters);
        }

        public bool CheckCurrData(ItemType it, ParamType pt)
        {
            if (!SimpleFilterCore.CheckCurrData(it, pt))
            {
                return false;
            }

            //2 通过过滤，识别当前对象的分类类型
            EnumType et = IdentifyItemTypeAsEnumType(it, pt);

            //3 设置当前检测条数
            CurrCheckedCount++;

            //4 根据分类类型，判断该对象是否应该添加
            if (!IsValidate(et))
            {
                return;
            }

            //6 设置当前分类的获取数目
            SetEachGetCount(et);
        }

        //清空现有数据状态
        /// <summary>
        /// 清空现有数据状态
        /// </summary>
        private void ClearData()
        {
            //当前检测的记录清0
            CurrCheckedCount = 0;

            //当前各个类型上已获取的个数 清0 
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                CurrEachTypeGetedCount[value] = 0;
            }
        }

        private bool IsValidate(EnumType needPolicyType)
        {
            foreach (EnumType enumType in Enum.GetValues(typeof(EnumType)))
            {
                if (((enumType.ChangeType<int>() & needPolicyType.ChangeType<int>()) == enumType.ChangeType<int>())
                    && CurrEachTypeGetedCount[enumType] < EachTypeMinGetCount[enumType])
                {
                    return true;
                }
            }
            return false;
        }

        //设置对应类型已经获取数
        /// <summary>
        /// 设置对应类型已经获取数
        /// </summary>
        /// <param name="sourceType"></param>
        private void SetEachGetCount(EnumType sourceType)
        {
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                SetEachGetCount(sourceType, value);
            }
        }

        //指定类型获取数+1
        /// <summary>
        /// 指定类型获取数+1
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        private void SetEachGetCount(EnumType sourceType, EnumType targetType)
        {
            if ((sourceType.ChangeType<int>() & targetType.ChangeType<int>()) == targetType.ChangeType<int>())
            {
                CurrEachTypeGetedCount[targetType]++;
            }
        }

        //数据初始化
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="identifyItemTypeAsEnumType"></param>
        /// <param name="currNeedType"></param>
        /// <param name="currEachGetCount"></param>
        /// <param name="maxCheckCount"></param>
        private void Init(Func<ItemType, ParamType, EnumType> identifyItemTypeAsEnumType, EnumType currNeedType, int currEachGetCount, int maxCheckCount)
        {
            //初始化需要类型     
            CurrNeedType = currNeedType;

            //初始化对象类型识别器
            IdentifyItemTypeAsEnumType = identifyItemTypeAsEnumType;

            //当前有效数据缓存
            CurrPickedData = new Dictionary<ItemType, EnumType>();

            //当前各个类型上已获取的个数
            CurrEachTypeGetedCount = new Dictionary<EnumType, int>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<EnumType, int>();

            Dictionary<EnumType, int> temp = new Dictionary<EnumType, int>();
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                temp.Add(value, currEachGetCount);
            }

            //初始化每种类型需要提取的个数
            foreach (KeyValuePair<EnumType, int> item in temp)
            {
                EachTypeMinGetCount[item.Key] = item.Value;
            }

            //初始化最多检测的对象数
            MaxCheckCount = maxCheckCount;
        }

    }
    //数据筛选基类
    /// <summary>
    /// 数据筛选基类
    /// </summary>
    /// <typeparam name="ItemType">待检测数据的数据类型</typeparam>
    /// <typeparam name="ParamType">辅助参数的数据类型</typeparam>
    /// <typeparam name="EnumType">数据项分类的枚举类型</typeparam>
    /// <typeparam name="StorItemType">存储的数据项类型</typeparam>
    public class FilterBaseCore<ItemType, ParamType, StorItemType, EnumType>
        where ItemType : class
        where ParamType : class
        where StorItemType : class
        where EnumType : struct
    {
        // 当前需要提取的类型
        /// <summary>
        /// 当前需要提取的类型
        /// </summary>
        private EnumType CurrNeedType { get; set; }

        //当前已经匹配的有效数据的条数
        /// <summary>
        /// 当前已经匹配的有效数据的条数
        /// </summary>
        private int CurrCheckedCount { get; set; }

        //最多匹配的有效数据的条数
        /// <summary>
        /// 最多匹配的有效数据的条数
        /// </summary>
        private int MaxCheckCount { get; set; }

        //每种类型需要提取的最小条数
        /// <summary>
        /// 每种类型需要提取的最小条数
        /// </summary>
        private Dictionary<EnumType, int> EachTypeMinGetCount { get; set; }

        //每种类型已经提取的条数
        /// <summary>
        /// 每种类型已经提取的条数
        /// </summary>
        private Dictionary<EnumType, int> CurrEachTypeGetedCount { get; set; }

        //退出类型
        /// <summary>
        /// 退出类型
        /// </summary>
        public FinishedType CurrFinishedType { get; set; }

        //是否已经执行过初始化函数
        /// <summary>
        /// 是否已经执行过初始化函数
        /// </summary>
        protected bool IsInited { get; set; }

        //当前检测的对象
        /// <summary>
        /// 当前检测的对象
        /// </summary>
        protected ItemType CurrItem { get; private set; }

        //获取是否已经结束
        /// <summary>
        /// 获取是否已经结束
        /// </summary>
        public bool IsFinished
        {
            get
            {
                //检测条数退出条件
                bool isNeedCountFinished = CurrCheckedCount >= MaxCheckCount;
                if (isNeedCountFinished)
                {
                    CurrFinishedType = FinishedType.CheckCount;
                    return true;
                }

                //类型个数退出条件
                bool isFinished = CurrEachTypeGetedCount.All(dic =>
                {
                    if ((dic.Key.ChangeType<int>() & CurrNeedType.ChangeType<int>()) == dic.Key.ChangeType<int>())
                    {
                        return dic.Value >= EachTypeMinGetCount[dic.Key];
                    }
                    return true;
                });
                if (isFinished)
                {
                    CurrFinishedType = FinishedType.NormalFinished;
                    return true;
                }

                //自定义退出条件
                bool customerCheckFinished = CustomerCheckFinished != null && CustomerCheckFinished.GetInvocationList().Any(x => x.DynamicInvoke(this).ChangeType<bool>());
                if (customerCheckFinished)
                {
                    CurrFinishedType = FinishedType.CustomerFinished;
                }
                return customerCheckFinished;
            }
        }

        //当前提出的数据集合
        /// <summary>
        /// 当前提出的数据集合
        /// </summary>
        public Dictionary<StorItemType, EnumType> CurrPickedData { get; set; }

        //自定义退出条件
        /// <summary>
        /// 自定义退出条件
        /// </summary>
        private Func<FilterBaseCore<ItemType, ParamType, StorItemType, EnumType>, bool> CustomerCheckFinished { get; set; }

        //自定义过滤器
        /// <summary>
        /// 自定义过滤器
        /// </summary>
        private Func<ItemType, ParamType, bool> CustomerFilter { get; set; }

        //类型转换器
        /// <summary>
        /// 类型转换器
        /// </summary>
        private Func<ItemType, ParamType, EnumType> IdentifyItemTypeAsEnumType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Func<ItemType, ParamType, StorItemType> GetStorItemType { get; set; }

        //添加数据项 完成后可以执行的操作
        /// <summary>
        /// 添加数据项 完成后可以执行的操作
        /// </summary>
        private Action<ItemType> ItemAddedCompleted { get; set; }


        //构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public FilterBaseCore(Func<ItemType, ParamType, EnumType> identifyItemTypeAsEnumType, Func<ItemType, ParamType, StorItemType> getStorItemType = null)
        {
            //当前有效数据缓存
            CurrPickedData = new Dictionary<StorItemType, EnumType>();

            //当前各个类型上已获取的个数
            CurrEachTypeGetedCount = new Dictionary<EnumType, int>();

            //每种类型需要提取的最小条数
            EachTypeMinGetCount = new Dictionary<EnumType, int>();

            //初始化对象类型识别器
            IdentifyItemTypeAsEnumType = identifyItemTypeAsEnumType;

            GetStorItemType = getStorItemType;
            IsInited = false;
        }


        //数据初始化
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="currNeedType"></param>
        /// <param name="currEachGetCount"></param>
        /// <param name="maxCheckCount"></param>
        public virtual void Init(EnumType currNeedType, int currEachGetCount, int maxCheckCount)
        {
            Dictionary<EnumType, int> temp = new Dictionary<EnumType, int>();
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                temp.Add(value, currEachGetCount);
            }
            Init(currNeedType, temp, maxCheckCount);
        }

        //数据初始化
        /// <summary>
        /// 数据初始化
        /// </summary>
        /// <param name="currNeedType"></param>
        /// <param name="eachTypeMinGetCount"></param>
        /// <param name="maxCheckCount"></param>
        public virtual void Init(EnumType currNeedType, Dictionary<EnumType, int> eachTypeMinGetCount, int maxCheckCount)
        {
            ClearData();

            //初始化自定义退出条件
            SetCustomerCheckFinished(null);

            //初始化自定义过滤器
            SetCustomerFilter(null);

            //初始化数据添加完成后操作
            SetItemAddedCompleted(null);

            //初始化需要类型     
            CurrNeedType = currNeedType;

            //初始化每种类型需要提取的个数
            foreach (KeyValuePair<EnumType, int> item in eachTypeMinGetCount)
            {
                EachTypeMinGetCount[item.Key] = item.Value;
            }
            //初始化最多检测的对象数
            MaxCheckCount = maxCheckCount;

            IsInited = true;
        }

        //清空现有数据状态
        /// <summary>
        /// 清空现有数据状态
        /// </summary>
        public virtual void ClearData()
        {
            //当前检测的记录清0
            CurrCheckedCount = 0;

            //清空当前已获取的数据
            CurrPickedData.Clear();

            //当前各个类型上已获取的个数 清0 
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                CurrEachTypeGetedCount[value] = 0;
            }
        }

        //检测当前政策行是否需要添加
        /// <summary>
        /// 检测当前政策行是否需要添加
        /// </summary>
        /// <param name="it"></param>
        /// <param name="pt"></param>
        public void CheckCurrData(ItemType it, ParamType pt)
        {
            if (!IsInited)
            {
                throw new Exception("初始化方法Init未被调用");
            }

            CurrItem = it;

            //1 过滤器过滤（过滤失败 包含false）
            if (CustomerFilter != null && CustomerFilter.GetInvocationList().Any(x => !x.DynamicInvoke(it, pt).ChangeType<bool>()))
            {
                return;
            }

            //2 通过过滤，识别当前对象的分类类型
            EnumType et = IdentifyItemTypeAsEnumType(it, pt);

            //3 设置当前检测条数
            CurrCheckedCount++;

            //4 根据分类类型，判断该对象是否应该添加
            if (IsValidate(et))
            {
                return;
            }

            //5 添加该条数据暂存
            if (GetStorItemType != null)
            {
                CurrPickedData.Add(GetStorItemType(it, pt), et);
            }

            if (ItemAddedCompleted != null)
            {
                ItemAddedCompleted(it);
            }

            //6 设置当前分类的获取数目
            SetEachGetCount(et);
        }

        //添加自定义退出检测方法
        /// <summary>
        /// 添加自定义退出检测方法
        /// </summary>
        /// <param name="customerCheckFinisheds"></param>
        public void SetCustomerCheckFinished(params Func<FilterBaseCore<ItemType, ParamType, StorItemType, EnumType>, bool>[] customerCheckFinisheds)
        {
            if (customerCheckFinisheds == null)
            {
                CustomerCheckFinished = null;
                return;
            }

            foreach (Func<FilterBaseCore<ItemType, ParamType, StorItemType, EnumType>, bool> customerCheckFinished in customerCheckFinisheds)
            {
                if (CustomerCheckFinished == null)
                {
                    CustomerCheckFinished = customerCheckFinished;
                }
                else
                {
                    CustomerCheckFinished += customerCheckFinished;
                }
            }
        }

        //添加自定义过滤器
        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="customerFilters"></param>
        public void SetCustomerFilter(params Func<ItemType, ParamType, bool>[] customerFilters)
        {
            if (customerFilters == null)
            {
                CustomerFilter = null;
                return;
            }
            foreach (Func<ItemType, ParamType, bool> customerFilter in customerFilters)
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

        //添加数据添加完成后需要执行的操作
        /// <summary>
        /// 添加数据添加完成后需要执行的操作
        /// </summary>
        /// <param name="itemAddedCompleted"></param>
        protected void SetItemAddedCompleted(Action<ItemType> itemAddedCompleted)
        {
            ItemAddedCompleted = itemAddedCompleted;
        }

        //验证当前政策的政策类型有效性
        /// <summary>
        /// 验证当前政策的政策类型有效性
        /// true 不需要添加当前政策 false 需要添加当前政策
        /// </summary>
        /// <param name="needPolicyType">当前政策的政策类型（高 低 当期 下期）</param>
        /// <returns></returns>
        private bool IsValidate(EnumType needPolicyType)
        {
            foreach (EnumType enumType in Enum.GetValues(typeof(EnumType)))
            {
                if ((enumType.ChangeType<int>() & needPolicyType.ChangeType<int>()) == enumType.ChangeType<int>() && CurrEachTypeGetedCount[enumType] < EachTypeMinGetCount[enumType])
                {
                    return false;
                }
            }
            return true;
        }

        //设置对应类型已经获取数
        /// <summary>
        /// 设置对应类型已经获取数
        /// </summary>
        /// <param name="sourceType"></param>
        private void SetEachGetCount(EnumType sourceType)
        {
            foreach (EnumType value in Enum.GetValues(typeof(EnumType)))
            {
                SetEachGetCount(sourceType, value);
            }
        }

        //指定类型获取数+1
        /// <summary>
        /// 指定类型获取数+1
        /// </summary>
        /// <param name="sourceType"></param>
        /// <param name="targetType"></param>
        [Background]
        private void SetEachGetCount(EnumType sourceType, EnumType targetType)
        {
            if ((sourceType.ChangeType<int>() & targetType.ChangeType<int>()) == targetType.ChangeType<int>())
            {
                CurrEachTypeGetedCount[targetType]++;
            }
        }

    }
}
