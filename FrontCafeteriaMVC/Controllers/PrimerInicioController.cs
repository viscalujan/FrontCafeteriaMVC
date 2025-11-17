using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    public class PrimerInicioController : Controller
    {
        private readonly IServicesAPI _api;

        public PrimerInicioController(IServicesAPI api)
        {
            _api = api;
        }

        // MOSTRAR FORMULARIO
        [HttpGet]
        public IActionResult Index()
        {
            var correo = TempData["Correo"] as string;

            if (string.IsNullOrWhiteSpace(correo))
                return RedirectToAction("Index", "Login");

            ViewBag.Correo = correo;
            TempData["Correo"] = correo;

            return View();
        }

        // ENVIAR CÓDIGO AL CORREO
        [HttpPost]
        public async Task<IActionResult> Index(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Ingresa un correo válido.";
                return View();
            }

            var ok = await _api.SolicitarCodigoRecuperacionAsync(correo);

            if (!ok)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "El correo no existe en el sistema.";
                ViewBag.Correo = correo;
                return View();
            }

            TempData["Correo"] = correo;
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Código enviado a tu correo.";

            // 🔥 AHORA SIGUE EL MISMO FLUJO DE RECUPERAR CONTRASEÑA
            return RedirectToAction("ValidarCodigo", "Recuperar");
        }
    }
}
