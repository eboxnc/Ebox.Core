
using CSRedis;
using Ebox.Core.Common.Helpers;
using Ebox.Core.Interface.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ebox.Core.Interface.Service
{
    public class RedisCache : ICacheService, ICacheManager
    {
        public static CSRedisClient RedisServer;
        static RedisCache()
        {
            RedisServer = new CSRedisClient(Appsettings.app(new string[] { "RedisServer", "Cache" }));
        }

        public void Add<V>(string key, V value)
        {
            RedisServer.Set(key, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="key"></param>
        /// <param name="value">过期时间：秒</param>
        /// <param name="cacheDurationInSeconds"></param>
        public void Add<V>(string key, V value, int cacheDurationInSeconds)
        {
            RedisServer.Set(key, value, cacheDurationInSeconds);
        }

        public bool ContainsKey<V>(string key)
        {
            return RedisServer.Exists(key);
        }

        public V Get<V>(string key)
        {
            return RedisServer.Get<V>(key);
        }

        public IEnumerable<string> GetAllKey<V>()
        {
            return RedisServer.Keys("Cache:SqlSugarDataCache.*");
        }

        public V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue)
        {
            if (ContainsKey<V>(cacheKey))
            {
                return Get<V>(cacheKey);
            }
            else
            {
                var result = create();
                Add(cacheKey, result, cacheDurationInSeconds);
                return result;
            }
        }

        public void Remove<V>(string key)
        {
            RedisServer.Del(key.Remove(0, 6));
        }
    }
}
