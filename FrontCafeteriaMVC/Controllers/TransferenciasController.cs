using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    public class TransferenciasController : Controller
    {
        private readonly IServicesAPI _api;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransferenciasController(IServicesAPI api, IHttpContextAccessor httpContextAccessor)
        {
            _api = api;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(TransferenciaCreditoDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                // 🔹 Obtener número de control del usuario logueado
                var numeroControlEmisor = _httpContextAccessor.HttpContext.Session.GetString("NumeroControl");

                if (string.IsNullOrEmpty(numeroControlEmisor))
                    return RedirectToAction("Login", "Auth");

                await _api.TransferirCreditoAsync(model, numeroControlEmisor);

                ViewBag.Mensaje = "✅ Transferencia completada con éxito.";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            return View();
        }
    }
}
