using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FrontCafeteriaMVC.Filters
{
    public class AuthorizeSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var token = session.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(
                    new { controller = "Auth", action = "Login" }
                ));
            }
            base.OnActionExecuting(context);
        }
    }
}
