using System.Collections.Generic;
using System.Linq;
using ServiceStack.Redis;

namespace DeanZhou.Framework
{
    public static class RedisTest
    {
        private static RedisConfig ServerConfig = new RedisConfig(@"C:\Program Files\Redis 2.4.5\64bit", "", 0, 0, 0);
        private static RedisConfig ClientConfig = new RedisConfig("", "127.0.0.1:6379|", isClient: true);

        public static void Test()
        {
            List<RedisConfig> ls = LocalDB<RedisConfig>.Select();
            if (ls.Exists(c => c.IsClient))
            {
                ClientConfig = ls.First(c => c.IsClient);
            }
            else
            {
                ClientConfig = new RedisConfig("", "127.0.0.1:6379|", isClient: true);
                LocalDB<RedisConfig>.Insert(ClientConfig);
            }

            if (ls.Exists(c => !c.IsClient))
            {
                ServerConfig = ls.First(c => !c.IsClient);
            }
            else
            {
                ServerConfig = new RedisConfig(@"C:\Program Files\Redis 2.4.5\64bit", "", 0, 0, 0);
                LocalDB<RedisConfig>.Insert(ServerConfig);
            }

            ServerConfig.OpenService();
            PooledRedisClientManager cm = ClientConfig.GetClientManager();
            IRedisClient client = cm.GetClient();
            client.Set("key1", "fasld;asdf");
            var res = client.GetValue("key1");
        }
    }
}
