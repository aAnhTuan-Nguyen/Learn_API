using TodoWeb.Application.Services.MiddlwareServices;

namespace TodoWeb.Application.Middleware
{
    public class BanTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ITokenBlacklistService _blacklistService;

        public BanTokenMiddleware(RequestDelegate next, ITokenBlacklistService blacklistService)
        {
            _next = next;
            _blacklistService = blacklistService;
        }

        public async Task Invoke(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
            {
                var token = authorizationHeader.Replace("Bearer ", "").Trim();

                if (_blacklistService.IsTokenBanned(token))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Access token has been banned.");
                    return; // Chặn request ngay lập tức
                }
            }

            await _next(context); // Nếu token hợp lệ, tiếp tục xử lý request
        }
    }
}
