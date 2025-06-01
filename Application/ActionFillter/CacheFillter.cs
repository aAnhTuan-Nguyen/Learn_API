using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using TodoWeb.Application.Services.CacheServices;

namespace TodoWeb.Application.ActionFillter
{
    public class CacheFillter : ActionFilterAttribute
    {
        //private readonly ICacheService _cacheService;
        private readonly IMemoryCache _cache;
        private readonly int _duration;
        private string key;
        public CacheFillter(int duration, IMemoryCache cache)
        {
            _cache = cache;
            _duration = duration;
        }

        // khi action được gọi thì sẽ kiểm tra xem có dữ liệu trong cache hay không
        // action meaning là phương thức trong controller
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            key = context.HttpContext.Request.Path.ToString();

            var cacheData = _cache.Get(key);
            if (cacheData != null)
            {
                context.Result = new ObjectResult(cacheData);
                return;
            }
            //if (_cache.TryGetValue(key, out var cacheData))
            //{
            //    context.Result = new ObjectResult(cacheData);
            //    return;
            //}
        }

        // khi action được thực thi xong thì sẽ lưu lại kết quả vào cache
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            
            var data = context.Result as ObjectResult;

            if (data != null)
            {
                var key = context.HttpContext.Request.Path.ToString();
                _cache.Set(key, data.Value, TimeSpan.FromSeconds(_duration));
            }
        }
    }
}
