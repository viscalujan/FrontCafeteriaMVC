using FrontCafeteriaMVC.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FrontCafeteriaMVC.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Index()
        {
            var exceptionHandlerPathFeature =
                HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            // Registra el error (opcional)
            if (exceptionHandlerPathFeature?.Error != null)
            {
                Console.WriteLine($"Error: {exceptionHandlerPathFeature.Error}");
            }

            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Exception = exceptionHandlerPathFeature?.Error
            });
        }
    }
}
