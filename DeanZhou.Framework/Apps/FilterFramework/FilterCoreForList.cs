using System;
using System.Collections.Generic;
using System.Linq;

namespace DeanZhou.Framework
{

    /// <summary>
    /// 列表过滤器
    /// </summary>
    /// <typeparam name="TItem">你需要过滤的对象的类型</typeparam>
    /// <typeparam name="TParam">过滤一个对象需要的辅助参数的类型</typeparam>
    /// <typeparam name="TItemType">为过滤对象进行类型分类的类型</typeparam>
    public class ListFilterCore<TItem, TParam, TItemType>
        where TItem : class
        where TParam : class
        where TItemType : struct
    {
        #region 构造函数

        /// <summary>
        /// 缓存需要识别类型的所有枚举值
        /// 静态构造函数中进行初始化
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

        #region Set MinGetCount

        /// <summary>
        /// 当前需要提取的类型 
        /// </summary>
        protected TItemType CurrNeedType;

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

            if (EachTypeMinGetCount.Any(c => c.Value > 0))
            {
                CurrNeedType = EachTypeMinGetCount.First(c => c.Value > 0).Key;
            }

            bool isFirst = true;
            foreach (KeyValuePair<TItemType, int> kv in EachTypeMinGetCount)
            {
                if (isFirst)
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = kv.Key;
                        isFirst = false;
                    }
                }
                else
                {
                    if (kv.Value > 0)
                    {
                        CurrNeedType = (CurrNeedType.GetValue<int>() | kv.Key.GetValue<int>()).GetValue<TItemType>();
                    }
                }
            }

            try
            {
                QuitValue = EachTypeMinGetCount.Values.Sum();
            }
            catch (Exception)
            {
                QuitValue = int.MaxValue;
            }
            return this;
        }

        #endregion

        #region Regist ItemTypeIdentifier

        /// <summary>
        /// 类型转换器 *
        /// </summary>
        protected Func<TItem, TParam, TItemType> ItemTypeIdentifier;

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="itemTypeIdentifier">类型识别器</param>
        public ListFilterCore<TItem, TParam, TItemType> RegistItemTypeIdentifier(Func<TItem, TParam, TItemType> itemTypeIdentifier)
        {
            //初始化对象类型识别器
            ItemTypeIdentifier = itemTypeIdentifier;
            return this;
        }

        /// <summary>
        /// 注册类型识别器
        /// </summary>
        /// <param name="itemTypeIdentifier"></param>
        /// <returns></returns>
        public ListFilterCore<TItem, TParam, TItemType> RegistItemTypeIdentifier(IItemTypeIdentifier<TItem, TParam, TItemType> itemTypeIdentifier)
        {
            if (itemTypeIdentifier == null)
            {
                return this;
            }
            //初始化对象类型识别器
            ItemTypeIdentifier = itemTypeIdentifier.IdentifyItemType;
            return this;
        }

        #endregion

        #region Add Filter

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

        #endregion

        #region Do Filter

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessItemList"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual List<TItem> DoFilter(IEnumerable<TItem> waitProcessItemList, TParam param)
        {
            return DoFilterWithType(waitProcessItemList, param).Keys.ToList();
        }

        /// <summary>
        /// 获取过滤后的数据
        /// </summary>
        /// <param name="waitProcessItemList"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Dictionary<TItem, TItemType> DoFilterWithType(IEnumerable<TItem> waitProcessItemList, TParam param)
        {
            if (waitProcessItemList == null)
            {
                return new Dictionary<TItem, TItemType>();
            }

            if (ItemTypeIdentifier == null)
            {
                throw new Exception("未注册类型识别器");
            }

            Dictionary<TItem, TItemType> res = new Dictionary<TItem, TItemType>();

            //当前各个类型上已获取的个数
            Dictionary<TItemType, int> currEachTypeGetedCount = new Dictionary<TItemType, int>();
            {
                foreach (TItemType item in EnumTypes)
                {
                    currEachTypeGetedCount[item] = 0;
                }
            }

            foreach (TItem item in waitProcessItemList)
            {
                //识别对象
                TItemType itemType = ItemTypeIdentifier(item, param);

                //识别出对象拆分（枚举原子化）
                IEnumerable<TItemType> validEnums = EnumTypes.Where(t => itemType.HasItem(t)).ToList();

                //当前类型已经取满 就不用再走过滤器了
                if (validEnums.All(t => currEachTypeGetedCount[t] >= EachTypeMinGetCount[t]))
                {
                    continue;
                }

                //使用简单过滤器删选
                if (!SimpleFilterCore.DoFilter(item, param))
                {
                    continue;
                }

                //更新当前分类的获取数目
                foreach (TItemType t in validEnums)
                {
                    currEachTypeGetedCount[t]++;
                }

                //添加结果
                res.Add(item, itemType);

                //是否已经完成
                if (IsFinished(res.Count, currEachTypeGetedCount))
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

            if (EachTypeMinGetCount.Any(c => c.Value > 0))
            {
                //类型个数退出条件
                bool isFinished = currEachTypeGetedCount.All(dic =>
                {
                    if (CurrNeedType.HasItem(dic.Key))
                    {
                        return dic.Value >= EachTypeMinGetCount[dic.Key];
                    }
                    return true;
                });
                return isFinished;
            }

            return false;
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
