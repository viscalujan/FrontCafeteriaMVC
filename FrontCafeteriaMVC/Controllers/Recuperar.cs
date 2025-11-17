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

        // ========== PASO 1: INGRESAR CORREO ==========
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string correo)
        {
            if (string.IsNullOrWhiteSpace(correo))
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Ingresa un correo electrónico.";
                return View();
            }

            var ok = await _api.SolicitarCodigoRecuperacionAsync(correo);

            if (!ok)
            {
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "No existe un usuario con ese correo.";
                return View();
            }

            TempData["Correo"] = correo;
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = $"Código enviado a {correo}.";

            return RedirectToAction("ValidarCodigo");
        }

        // ========== PASO 2: VALIDAR CÓDIGO ==========
        [HttpGet]
        public IActionResult ValidarCodigo()
        {
            var correo = TempData["Correo"] as string;

            if (string.IsNullOrWhiteSpace(correo))
                return RedirectToAction("Index");

            ViewBag.Correo = correo;
            TempData["Correo"] = correo; // lo volvemos a guardar para el POST

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ValidarCodigo(string correo, string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 6)
            {
                ViewBag.Correo = correo;
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "Código incompleto. Ingresa los 6 dígitos.";
                return View();
            }

            var ok = await _api.ValidarCodigoRecuperacionAsync(correo, codigo);

            if (!ok)
            {
                ViewBag.Correo = correo;
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Código incorrecto o expirado.";
                return View();
            }

            TempData["Correo"] = correo;
            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Código verificado correctamente.";

            return RedirectToAction("NuevaContra");
        }

        // ========== PASO 3: NUEVA CONTRASEÑA ==========
        [HttpGet]
        public IActionResult NuevaContra()
        {
            var correo = TempData["Correo"] as string;
            if (string.IsNullOrWhiteSpace(correo))
                return RedirectToAction("Index");

            ViewBag.Correo = correo;
            TempData["Correo"] = correo; // conservar para el POST

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NuevaContra(string correo, string nueva, string confirmacion)
        {
            if (string.IsNullOrWhiteSpace(nueva) || string.IsNullOrWhiteSpace(confirmacion))
            {
                ViewBag.Correo = correo;
                TempData["ToastType"] = "warning";
                TempData["ToastMessage"] = "Debes llenar ambos campos de contraseña.";
                return View();
            }

            if (nueva != confirmacion)
            {
                ViewBag.Correo = correo;
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "Las contraseñas no coinciden.";
                return View();
            }

            var ok = await _api.GuardarNuevaContraAsync(correo, nueva);

            if (!ok)
            {
                ViewBag.Correo = correo;
                TempData["ToastType"] = "error";
                TempData["ToastMessage"] = "No se pudo actualizar la contraseña.";
                return View();
            }

            TempData["ToastType"] = "success";
            TempData["ToastMessage"] = "Contraseña actualizada correctamente. Ahora puedes iniciar sesión.";

            return RedirectToAction("Index", "Login");
        }
    }
}
