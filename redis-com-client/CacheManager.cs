using System;
using System.EnterpriseServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace redis_com_client
{
    [ComVisible(true)]
    [Guid("6e8c90dd-15b6-4ee9-83c1-f294d6dca2a8")]
    [ClassInterface(ClassInterfaceType.None)]
    [ProgId("CacheManager")]
    [Synchronization(SynchronizationOption.Disabled)]
    public class CacheManager : ICacheManager
    {
        private string _storePrefix;

        public CacheManager()
        {
        }

        public void SetExpiration(string key, int milliseconds)
        {
            CacheFactory.GetInstance().KeyExpire(GenerateFullKey(key), TimeSpan.FromMilliseconds(milliseconds));
        }

        public void RemoveAll()
        {
            var mask = $"{_storePrefix}*";
            CacheFactory.GetInstance().ScriptEvaluate("local keys = redis.call('keys', ARGV[1]) for i=1,#keys,5000 do redis.call('del', unpack(keys, i, math.min(i+4999, #keys))) end return keys", null, new RedisValue[] { mask });
        }

        public object Get(string key)
        {
            var fullKey = GenerateFullKey(key);
            string pair = CacheFactory.GetInstance().StringGet(fullKey);

            if (string.IsNullOrEmpty(pair))
                return null;

            if (!pair.Contains("ArrayCollumn"))
                return pair;
                
            var table = JsonConvert.DeserializeObject<MyTable>(pair);
            try { 
                return (object[,])table.GetArray();
            }
            catch (Exception) { 
                return (object[])table.GetArray();
            }
        }


        public void Add(string key, object value)
        {
            Add(key, value, 0);
        }

        private void Add(string key, object value, int millisecondsToExpire)
        {
            object valueToAdd = value?.ToString() ?? string.Empty;
            var fullKey = GenerateFullKey(key);

            if (value != null && value.GetType().IsArray)
            {
                try
                {
                    var array = (object[,])value;

                    var table = new MyTable(array);
                    valueToAdd = JsonConvert.SerializeObject(table);
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("cast object", StringComparison.InvariantCultureIgnoreCase) > 0) //most likely the array is not bi-dimensional, try again with only 1 dimenion
                    {
                        var array = (object[])value;

                        var table = new MyTable(array);
                        valueToAdd = JsonConvert.SerializeObject(table);
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            if (millisecondsToExpire > 0)
            {
                CacheFactory.GetInstance().StringSet(fullKey, (string)valueToAdd, TimeSpan.FromMilliseconds(millisecondsToExpire));
            }
            else
            {
                CacheFactory.GetInstance().StringSet(fullKey, (string)valueToAdd);
            }
        }

        public object this[string key]
        {
            get { return Get(key); }
            set { Add(key, value); }
        }

        public void Init(string cacheId)
        {
            _storePrefix = string.Concat(cacheId, ":");
        }

        private string GenerateFullKey(string key)
        {
            if (string.IsNullOrEmpty(_storePrefix))
                throw new Exception("no cache key defined - operation not allowed.");

            return (string.Concat(_storePrefix, key));
        }

        public void Remove(string key)
        {
            CacheFactory.GetInstance().KeyDelete(GenerateFullKey(key));
        }

        public bool Exists(string key)
        {
            return CacheFactory.GetInstance().KeyExists(GenerateFullKey(key));
        }
    }
}