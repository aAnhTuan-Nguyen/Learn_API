using Microsoft.Extensions.Caching.Memory;

namespace TodoWeb.Application.Services.MiddlwareServices
{
    public class TokenBlacklistService: ITokenBlacklistService
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _banDuration = TimeSpan.FromHours(1); // Token bị ban trong 1 giờ

        public TokenBlacklistService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void BanToken(string token)
        {
            _cache.Set(token, true, _banDuration);
        }

        public bool IsTokenBanned(string token)
        {
            return _cache.TryGetValue(token, out _);
        }

    }
}
