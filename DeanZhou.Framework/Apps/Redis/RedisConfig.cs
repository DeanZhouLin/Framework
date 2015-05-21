﻿using System.Linq;
using JFx;

namespace DeanZhou.Framework
{
    /// <summary>
    /// Redis配置
    /// </summary>
    public class RedisConfig
    {
        /// <summary>
        /// 服务路径
        /// </summary>
        public string ServerPath { get; private set; }

        /// <summary>
        /// 服务地址端口 如127.0.0.1:45632
        /// </summary>
        public string[] ReadWriteHosts { get; private set; }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public long DefaultDb { get; private set; }

        /// <summary>
        /// 最大写入线程数量
        /// </summary>
        public int MaxWritePoolSize { get; private set; }

        /// <summary>
        /// 最大读取线程数量
        /// </summary>
        public int MaxReadPoolSize { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsClient { get; private set; }

        public RedisConfig(
            string servicePath = @"C:\Program Files\Redis 2.4.5\64bit",
            string hosts = "127.0.0.1:45632|",
            long defaultDb = 8,
            int maxWritePoolSize = 100,
            int maxReadPoolSize = 100,
            bool isClient = false)
        {
            ServerPath = servicePath;
            ReadWriteHosts = hosts.Split('|').Where(c => !string.IsNullOrEmpty(c)).ToArray();
            DefaultDb = defaultDb;
            MaxWritePoolSize = maxWritePoolSize;
            MaxReadPoolSize = maxReadPoolSize;
            IsClient = isClient;
        }

        public override string ToString()
        {
            string str = string.Format("{0} {1} {2} {3} {4} {5}", ServerPath, string.Join(",", ReadWriteHosts), DefaultDb, MaxWritePoolSize, MaxReadPoolSize, IsClient);
            return str;
        }

        public string GetMD5Key()
        {
            return EncryptionHelper.Md5(ToString());
        }
    }
}