using StackExchange.Redis;

namespace redis_com_client
{
    public class CacheFactory
    {
        private static ConnectionMultiplexer _redisClientsManager;


        private CacheFactory()
        {
            
        }

        public static IDatabase GetInstance()
        {
            if (_redisClientsManager == null)
                _redisClientsManager = ConnectionMultiplexer.Connect("localhost");

            return _redisClientsManager.GetDatabase();
        }

        public static IServer GetServer()
        {
            if (_redisClientsManager == null)
                _redisClientsManager = ConnectionMultiplexer.Connect("localhost");

            return _redisClientsManager.GetServer("localhost", 6379);
        }
    }
}