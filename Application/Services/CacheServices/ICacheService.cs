namespace TodoWeb.Application.Services.CacheServices
{
    public interface ICacheService
    {
        public CacheData Get(string key);
        public void Set(string key, object value, int duaration);
        public void Remove(string key);
    }
}
