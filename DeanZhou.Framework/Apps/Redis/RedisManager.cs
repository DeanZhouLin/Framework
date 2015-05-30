using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ServiceStack.Redis;

namespace DeanZhou.Framework
{
    public static class RedisManager
    {
        private const string SERVICE_NAME = "redis-server.exe";
        private const string CONFIG_NAME = "redis.conf";

        private static readonly List<string> _hostedService = new List<string>();
        private static readonly Dictionary<string, PooledRedisClientManager> _clients = new Dictionary<string, PooledRedisClientManager>();

        /// <summary>
        /// 打开Redis数据库服务, 建立服务地址127.0.0.1:45632
        /// </summary>
        public static void OpenService(this RedisConfig config)
        {
            string key = config.MD5Key;
            if (_hostedService.Contains(key))
            {
                return;
            }

            if (config.IsClient)
            {
                throw new Exception("不是服务端");
            }

            if (!Directory.Exists(config.ServerPath) || !File.Exists(config.ServerPath + "\\" + SERVICE_NAME))
            {
                throw new ArgumentException("Redis server 开启失败,　路径" + config.ServerPath + "不存在" + SERVICE_NAME);
            }

            var processes = Process.GetProcessesByName(SERVICE_NAME);

            if (processes.Length > 0) return;
            var p = new Process
            {
                StartInfo =
                {
                    //创建CMD.EXE 进程
                    FileName = "CMD.EXE",
                    //重定向输入
                    RedirectStandardInput = true,
                    //重定向输出
                    RedirectStandardOutput = true,
                    // 不调用系统的Shell
                    UseShellExecute = false,
                    // 重定向Error
                    RedirectStandardError = true,
                    //不创建窗口
                    CreateNoWindow = true
                }
            }; // 初始化新的进程
            p.Start(); // 启动进程
            p.StandardInput.WriteLine("\"{0}\\{1}\" \"{0}\\{2}\"", config.ServerPath, SERVICE_NAME, CONFIG_NAME); // Cmd 命令
            _hostedService.Add(key);
        }

        /// <summary>
        /// 创建客户端配置管理对象
        /// </summary>
        /// <returns></returns>
        public static PooledRedisClientManager GetClientManager(this RedisConfig config)
        {
            string key = config.MD5Key;
            if (_clients.ContainsKey(key))
            {
                return _clients[key];
            }

            //支持读写分离，均衡负载
            var res = new PooledRedisClientManager(config.ReadWriteHosts, config.ReadWriteHosts, new RedisClientManagerConfig
            {
                DefaultDb = config.DefaultDb,
                MaxWritePoolSize = config.MaxWritePoolSize,
                MaxReadPoolSize = config.MaxWritePoolSize,
                AutoStart = true
            });
            _clients.Add(key, res);
            return res;
        }
    }
}
