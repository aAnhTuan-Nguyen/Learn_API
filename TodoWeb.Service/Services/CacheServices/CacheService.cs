namespace TodoWeb.Application.Services.CacheServices
{
    public class CacheService : ICacheService
    {
        private readonly Dictionary<string, CacheData> _cache = new Dictionary<string, CacheData>();

        public CacheData Get(string key)
        {
            //if (_cache.TryGetValue(key, out var cacheData) && cacheData.Expiration > DateTime.UtcNow)
            //{
            //    return cacheData;
            //}
            //return null;

            if (!_cache.ContainsKey(key))
            {
                return null;
            }
            var cacheData = _cache[key];
            if (cacheData.Expiration < DateTime.UtcNow)
            {
                _cache.Remove(key);
                return null;
            }
            return cacheData;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        public void Set(string key, object value, int duaration)
        {
            //var expiration = DateTime.UtcNow.AddMinutes(duaration);
            //var cacheData = new CacheData(value, expiration);

            var cacheData = new CacheData(value, DateTime.UtcNow.AddMinutes(duaration));
            _cache[key] = cacheData;
        }
    }
}
