using FrontCafeteriaMVC.Filters;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    [AuthorizeSession]
    public class UsuariosController : Controller
    {
        private readonly IServicesAPI _servicesApi;

        public UsuariosController(IServicesAPI servicesApi)
        {
            _servicesApi = servicesApi;
        }

        public async Task<IActionResult> Index()
        {
            var usuariosDTO = await _servicesApi.GetUsuariosAsync();
            return View(usuariosDTO);
        }

        public IActionResult Crear()
        {
            return View(new UsuarioRegistroDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            if (!ModelState.IsValid)
                return View(usuarioRegistroDTO);

            var resultado = await _servicesApi.RegistrarUsuarioAsync(usuarioRegistroDTO);

            if (resultado)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario.");
            return View(usuarioRegistroDTO);
        }

        [HttpGet]
        public IActionResult AumentarCredito()
        {
            return View(new AumentoCreditoDTO());
        }

        [HttpPost]
        public async Task<IActionResult> AumentarCredito(AumentoCreditoDTO dto)
        {
            if (dto.Cantidad < 50)
            {
                ModelState.AddModelError(string.Empty, "La cantidad debe ser al menos 50.");
                return View(dto);
            }

            var usuario = await _servicesApi.GetUsuarioPorNumeroControlAsync(dto.NumeroControl);

            if (usuario == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return View(dto);
            }

            var creditoAntes = usuario.Credito;

            var exito = await _servicesApi.AumentarCreditoAsync(dto);

            if (!exito)
            {
                TempData["Error"] = "No se pudo aumentar el crédito.";
                return View(dto);
            }

            // Consultamos de nuevo para obtener el nuevo crédito
            var usuarioActualizado = await _servicesApi.GetUsuarioPorNumeroControlAsync(dto.NumeroControl);

            ViewBag.CreditoAntes = creditoAntes;
            ViewBag.CreditoDespues = usuarioActualizado?.Credito ?? creditoAntes;

            TempData["Exito"] = "Crédito aumentado correctamente.";
            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerCredito(string numeroControl)
        {
            if (string.IsNullOrWhiteSpace(numeroControl))
                return Json(new { success = false, mensaje = "Número de control vacío." });

            var usuario = await _servicesApi.GetUsuarioPorNumeroControlAsync(numeroControl);

            if (usuario == null)
                return Json(new { success = false, mensaje = "Usuario no encontrado." });

            return Json(new { success = true, credito = usuario.Credito });
        }

        [HttpGet]
        public async Task<IActionResult> HistorialCredito(string numeroControl)
        {
            List<HistorialCredito> historial;

            if (string.IsNullOrWhiteSpace(numeroControl))
            {
                historial = await _servicesApi.ObtenerHistorialCreditoGeneralAsync();
            }
            else
            {
                historial = await _servicesApi.ObtenerHistorialCreditoAsync(numeroControl);
            }

            var modelo = new HistorialCreditoFiltroViewModel
            {
                NumeroControl = numeroControl,
                Historial = historial
            };

            return View(modelo);
        }


        [HttpGet]
        public IActionResult PagarLiquidacion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PagarLiquidacion(PagoLiquidacionDTO dto)
        {
            var resultado = await _servicesApi.PagarLiquidacionAsync(dto);
            ViewBag.Mensaje = resultado;
            return View();
        }

        public async Task<IActionResult> MiCuenta()
        {
            var numeroControl = User.Claims.FirstOrDefault(c => c.Type == "NumeroControl")?.Value;
            var nombre = User.Identity?.Name;

            if (string.IsNullOrWhiteSpace(numeroControl))
                return RedirectToAction("Index", "Login");

            var credito = await _servicesApi.ObtenerCreditoAsync(numeroControl) ?? 0;
            var historial = await _servicesApi.ObtenerHistorialCreditoAsync(numeroControl);

            var (base64, url) = await _servicesApi.ObtenerQrAsync(numeroControl);

            var vm = new MiCuentaVM
            {
                NumeroControl = numeroControl,
                Nombre = nombre,
                Credito = credito,
                Historial = historial.Select(h => new HistorialCreditoVM
                {
                    Fecha = h.Fecha,
                    Cantidad = h.Cantidad,
                    AutCorreo = h.AutCorreo
                }).ToList(),
                QrBase64 = base64,             // 👉 sin el prefijo aquí
                QrDownloadUrl = url
            };

            return View(vm);
        }


    }
}
