using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace TodoWeb.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
            {
            var exceptionHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandler?.Error.Message;

            _logger.LogError(exception);
            return new JsonResult(new
            {
                StatusCode = 500,
                Message = exception,
            });
        }
    }
}
