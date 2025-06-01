using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoWeb.Application.ActionFillter
{
    // ActionFilter nó là phiên bản mở rộng của IResultFilter và IActionFilter
    // nó có thể dùng để thực hiện các hành động trước và sau khi action được thực thi
    // và cũng có thể dùng để thực hiện các hành động trước và sau khi kết quả được trả về
    public class AuditFillter : ActionFilterAttribute
    {
        // lấy được tất cả thứ namgừ trong HttpRequest
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;

            var method = request.Method;

            var path = request.Path;

            var args = context.ActionArguments;

        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            var result = context.Result;

            var statusCode = context.HttpContext.Response.StatusCode;
        }
    }
}
