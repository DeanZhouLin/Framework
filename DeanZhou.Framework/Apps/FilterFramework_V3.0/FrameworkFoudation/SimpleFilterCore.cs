using System;
using System.Linq;

namespace DeanZhou.Framework
{

    /// <summary>
    /// 简单过滤器
    /// 过滤单个元素（无参）
    /// </summary>
    /// <typeparam name="TItemType">对象识别类型</typeparam>
    public sealed class SimpleFilterCore<TItemType>
    where TItemType : class
    {
        /// <summary>
        /// 自定义过滤器
        /// </summary>
        private Func<TItemType, bool> Filters { get; set; }

        /// <summary>
        /// 检测当前政策行是否需要添加
        /// </summary>
        /// <param name="it"></param>
        public bool CheckCurrData(TItemType it)
        {
            return Filters == null || Filters.GetInvocationList().All(x => x.DynamicInvoke(it).ChangeType<bool>());
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public SimpleFilterCore<TItemType> AddFilter(params Func<TItemType, bool>[] filters)
        {
            if (filters == null)
            {
                Filters = null;
                return this;
            }
            foreach (Func<TItemType, bool> customerFilter in filters)
            {
                if (Filters == null)
                {
                    Filters = customerFilter;
                }
                else
                {
                    Filters += customerFilter;
                }
            }
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        public SimpleFilterCore<TItemType> AddFilter(params IFilter<TItemType>[] filters)
        {
            if (filters == null)
            {
                return this;
            }
            foreach (var customerFilter in filters)
            {
                AddFilter(customerFilter.DoFilter);
            }
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="filterFullClassNames"></param>
        /// <returns></returns>
        public SimpleFilterCore<TItemType> AddFilter(string assemblyName, params string[] filterFullClassNames)
        {
            if (filterFullClassNames == null)
            {
                return this;
            }
            foreach (var fullClassName in filterFullClassNames)
            {
                var instance = Common.CreateIFilter<TItemType>(assemblyName, fullClassName);
                AddFilter(instance);
            }
            return this;
        }

    }

    /// <summary>
    /// 简单过滤器
    /// 过滤单个元素（含参）
    /// </summary>
    /// <typeparam name="TItemType">对象识别枚举数据类型</typeparam>
    /// <typeparam name="TParamType">辅助参数类型</typeparam>
    public sealed class SimpleFilterCore<TItemType, TParamType>
        where TItemType : class
        where TParamType : class
    {
        /// <summary>
        /// 自定义过滤器
        /// </summary>
        private Func<TItemType, TParamType, bool> Filters { get; set; }

        /// <summary>
        /// 检测当前政策行是否需要添加
        /// </summary>
        /// <param name="it"></param>
        /// <param name="pt"></param>
        public bool CheckCurrData(TItemType it, TParamType pt)
        {
            return Filters == null || Filters.GetInvocationList().All(x => x.DynamicInvoke(it, pt).ChangeType<bool>());
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        public SimpleFilterCore<TItemType, TParamType> AddFilter(params Func<TItemType, TParamType, bool>[] filters)
        {
            if (filters == null)
            {
                Filters = null;
                return this;
            }
            foreach (Func<TItemType, TParamType, bool> filter in filters)
            {
                if (Filters == null)
                {
                    Filters = filter;
                }
                else
                {
                    Filters += filter;
                }
            }
            return this;
        }

        /// <summary>
        /// 添加自定义过滤器
        /// </summary>
        /// <param name="filters"></param>
        public SimpleFilterCore<TItemType, TParamType> AddFilter(params IFilter<TItemType, TParamType>[] filters)
        {
            if (filters == null)
            {
                return this;
            }
            foreach (var filter in filters)
            {
                AddFilter(filter.DoFilter);
            }
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <param name="filterFullClassNames"></param>
        /// <returns></returns>
        public SimpleFilterCore<TItemType, TParamType> AddFilter(string assemblyName, params string[] filterFullClassNames)
        {
            if (filterFullClassNames == null)
            {
                return this;
            }
            foreach (var fullClassName in filterFullClassNames)
            {
                var instance = Common.CreateIFilter<TItemType, TParamType>(assemblyName, fullClassName);
                AddFilter(instance);
            }
            return this;
        }

    }

}
