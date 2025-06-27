using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoWeb.Application.ActionFillter
{
    public class LogFillter : IExceptionFilter
    {
        private readonly ILogger<LogFillter> _logger;
        private readonly LogLevel _logLevel;
        public LogFillter(ILogger<LogFillter> logger, LogLevel logLevel)
        {
            _logger = logger;
            _logLevel = logLevel;
        }
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            var message = $"Exception occurred: {exception.Message}";
            // có thể dùng @"" để format string
            
            _logger.Log(_logLevel, message);

            // context.Result để modify kết quả trả về cho client
            context.Result = new ObjectResult(new
            {
                message = @"An error occurred while processing your request.
                            Please contact with admin for more information",
                error = exception.Message
            })
            {
                StatusCode = 500
            };

            context.Result = new ObjectResult(new
            {
                message = "Buggggg",
                error = context.Exception.Message
            })
            { StatusCode = 500 };
        }
    }
}
