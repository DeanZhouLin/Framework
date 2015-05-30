using System;
using System.Collections.Generic;
using System.Dynamic;

namespace DeanZhou.Framework
{
    public class RequestBase
    {

        #region LocalCache

        /// <summary>
        /// 实际存储的缓存数据
        /// </summary>
        private readonly Dictionary<string, object> _dic = new Dictionary<string, object>();

        /// <summary>
        /// 本地缓存
        /// </summary>
        private dynamic _localCache;

        /// <summary>
        /// 本地缓存
        /// </summary>
        public dynamic LocalCache
        {
            get { return _localCache ?? (_localCache = new LocalCacheContainer(_dic)); }
        }

        /// <summary>
        /// 绑定本地缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="getValueFunc"></param>
        /// <param name="ps"></param>
        public void BindLocalCache(string key, Func<object[], object> getValueFunc, params object[] ps)
        {
            LocalCache.BindValue(key, getValueFunc, ps);
        }

        /// <summary>
        /// 本地缓存是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsExistLocalCache(string key)
        {
            return _dic.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        public bool IsExistLocalCache<T>(string key, out T res) where T : class
        {
            if (_dic.ContainsKey(key))
            {
                res = GetLocalCache<T>(key);
                return true;
            }
            res = default(T);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetLocalCache<T>(string key) where T : class
        {
            object res = GetLocalCache(key);
            if (res == null)
            {
                return default(T);
            }

            T t = res as T;
            if (t != null)
            {
                return t;
            }

            return res.ChangeType<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetLocalCache(string key)
        {
            if (_dic.ContainsKey(key))
            {
                return _dic[key];
            }
            return null;
        }

        /// <summary>
        /// 本地缓存容器
        /// </summary>
        class LocalCacheContainer : DynamicObject
        {
            private readonly Dictionary<string, object> _dic;

            public LocalCacheContainer(Dictionary<string, object> dic)
            {
                _dic = dic;
            }

            private void BindValue(string key, Func<object[], object> func, params object[] ps)
            {
                _dic[key] = func(ps);
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                if (binder.Name == "BindValue" && !_dic.ContainsKey(args[0].ToString()))
                {
                    Func<object[], object> func = args[1] as Func<object[], object>;
                    object[] ps = args[2] as object[];
                    BindValue(args[0].ToString(), func, ps);
                }
                result = null;
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var name = binder.Name;
                _dic.TryGetValue(name, out result);
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                return true;
            }
        }

        #endregion
    }
}
