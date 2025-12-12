using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Security.Claims;

namespace FrontCafeteriaMVC.Filters
{
    /// <summary>
    /// Valida que el usuario tenga alguno de los roles indicados.
    /// Usa el ClaimTypes.Role que se genera en el LoginController.
    /// </summary>
    public class AuthorizeRoleAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.User;

            // Si no está autenticado, lo mandamos al login
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Index", "Login", null);
                return;
            }

            var rol = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Si no tiene rol o no está dentro de los permitidos → lo mandamos a Home
            if (string.IsNullOrEmpty(rol) || !_roles.Contains(rol))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
