using System;
using System.Collections.Generic;

namespace Ebox.Core.Interface.IService
{
    public interface ICacheManager
    {
        void Add<V>(string key, V value);
        void Add<V>(string key, V value, int cacheDurationInSeconds);
        bool ContainsKey<V>(string key);
        V Get<V>(string key);
        IEnumerable<string> GetAllKey<V>();
        V GetOrCreate<V>(string cacheKey, Func<V> create, int cacheDurationInSeconds = int.MaxValue);
        void Remove<V>(string key);
    }
}
