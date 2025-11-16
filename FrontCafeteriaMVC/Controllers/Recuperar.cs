using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    public class RecuperarController : Controller
    {
        private readonly IServicesAPI _api;

        public RecuperarController(IServicesAPI api)
        {
            _api = api;
        }

        // 1) VISTA: ingresar correo
        public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Index(string correo)
        {
            var ok = await _api.SolicitarCodigoRecuperacionAsync(correo);

            if (!ok)
            {
                ViewBag.Error = "No se pudo enviar el código. Verifica tu correo.";
                return View();
            }

            TempData["Correo"] = correo;
            return RedirectToAction("ValidarCodigo");
        }

        // 2) VISTA: ingresar código
        public IActionResult ValidarCodigo()
        {
            if (TempData["Correo"] == null) return RedirectToAction("Index");
            ViewBag.Correo = TempData["Correo"]!.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarCodigo(string correo, string codigo)
        {
            var ok = await _api.ValidarCodigoRecuperacionAsync(correo, codigo);

            if (!ok)
            {
                ViewBag.Error = "Código incorrecto o expirado.";
                ViewBag.Correo = correo;
                return View();
            }

            TempData["Correo"] = correo;
            return RedirectToAction("NuevaContra");
        }

        // 3) VISTA: nueva contraseña
        public IActionResult NuevaContra()
        {
            if (TempData["Correo"] == null) return RedirectToAction("Index");
            ViewBag.Correo = TempData["Correo"]!.ToString();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevaContra(string correo, string nueva)
        {
            var ok = await _api.GuardarNuevaContraAsync(correo, nueva);

            if (!ok)
            {
                ViewBag.Error = "No se pudo cambiar la contraseña.";
                ViewBag.Correo = correo;
                return View();
            }

            TempData["Exito"] = "Contraseña cambiada exitosamente.";
            return RedirectToAction("Index", "Login");
        }
    }
}
