using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TodoWeb.Application.ActionFillter
{
    public class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly string[] _roles;
        public AuthorizationFilter(string roles)
        {
            _roles = roles.Split(',');
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var userId = context.HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                context.Result = new StatusCodeResult(401);
            }
            var role = context.HttpContext.Session.GetString("Role");

            if (!_roles.Contains(role))
            {
                context.Result = new StatusCodeResult(403);
            }
        }
    }
}
