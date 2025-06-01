
namespace TodoWeb.Application.Middleware
{
    // 30s , he thong chi cho phep 10 request
    public class RateLimitMiddleware : IMiddleware
    {
        private int count = 0;
        private DateTime startTime = DateTime.Now;

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            count++;
            var timeSpan = DateTime.Now - startTime;
            if (timeSpan.TotalSeconds > 30)
            {
                count = 0;
                startTime = DateTime.Now;
            }
            if (count > 10)
            {
                context.Response.StatusCode = 429; // Too Many Requests
                await context.Response.WriteAsync("Rate limit exceeded. Try again later.");
                return;
            }

            await next(context);
        }
    }
}
