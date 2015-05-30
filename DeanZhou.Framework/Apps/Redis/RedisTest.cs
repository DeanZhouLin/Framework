using ServiceStack.Redis;

namespace DeanZhou.Framework
{
    public static class RedisTest
    {
        private static readonly RedisConfig ServerConfig = new RedisConfig(@"C:\Program Files\Redis 2.4.5\64bit", "", 0, 0, 0);
        private static readonly RedisConfig ClientConfig = new RedisConfig("", "127.0.0.1:6379|", isClient: true);

        public static void Test()
        {
            ServerConfig.OpenService();
            PooledRedisClientManager cm = ClientConfig.GetClientManager();
            IRedisClient client = cm.GetClient();
            client.Add("key", "fasld;asdf");
            var res = client.GetValue("key");
        }
    }
}
