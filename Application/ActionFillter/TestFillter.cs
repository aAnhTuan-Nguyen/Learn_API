using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoWeb.Application.ActionFillter
{
    public class TestFillter : IActionFilter, IResultFilter
    {
        // chạy sau khi action được thực thi
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine("OnactionExecuted");
        }

        // chạy trước khi action được thực thi
        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine("OnActionExecuting");
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            Console.WriteLine("OnResultExecuted");
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            Console.WriteLine("OnResultExecuting");
        }
    }
    
}
