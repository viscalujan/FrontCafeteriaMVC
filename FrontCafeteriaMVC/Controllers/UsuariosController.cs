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
            var usuarios = await _servicesApi.GetUsuariosAsync();
            var viewModel = new UsuarioIndexViewModel
            {
                Usuarios = usuarios,
                UsuarioRegistro = new UsuarioRegistroDTO()
            };
            return View(viewModel);
        }

        public IActionResult Crear()
        {
            return View(new UsuarioRegistroDTO());
        }

        [HttpPost]

        public async Task<IActionResult> Crear([FromForm] UsuarioRegistroDTO usuarioRegistro)
        {
            var model = new UsuarioIndexViewModel
            {
                UsuarioRegistro = usuarioRegistro,
                Usuarios = await _servicesApi.GetUsuariosAsync()
            };

            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            // Validación adicional para el crédito mínimo
            if (usuarioRegistro.Credito < 50)
            {
                ModelState.AddModelError("UsuarioRegistro.Credito", "El crédito inicial debe ser al menos $50");
                return View("Index", model);
            }

            var resultado = await _servicesApi.RegistrarUsuarioAsync(usuarioRegistro);

            if (resultado)
            {
                TempData["Exito"] = "Usuario registrado correctamente";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario. Puede que el correo ya esté en uso.");
            return View("Index", model);
        }

        [HttpGet]
        public IActionResult AumentarCredito()
        {
            return View(new AumentoCreditoDTO());
        }


        [HttpPost]
        public async Task<IActionResult> AumentarCredito(UsuarioIndexViewModel model)
        {
            var dto = model.AumentoCredito;

            if (dto.Cantidad < 50)
            {
                ModelState.AddModelError(string.Empty, "La cantidad debe ser al menos 50.");
                model.Usuarios = await _servicesApi.GetUsuariosAsync();
                return View("Index", model);
            }

            var usuario = await _servicesApi.GetUsuarioPorNumeroControlAsync(dto.NumeroControl);

            if (usuario == null)
            {
                ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                model.Usuarios = await _servicesApi.GetUsuariosAsync();
                return View("Index", model);
            }

            var creditoAntes = usuario.Credito;

            var exito = await _servicesApi.AumentarCreditoAsync(dto);

            if (!exito)
            {
                ModelState.AddModelError(string.Empty, "No se pudo aumentar el crédito.");
                model.Usuarios = await _servicesApi.GetUsuariosAsync();
                return View("Index", model);
            }

            var usuarioActualizado = await _servicesApi.GetUsuarioPorNumeroControlAsync(dto.NumeroControl);

            TempData["Exito"] = $"Crédito aumentado correctamente. Antes: {creditoAntes:C}, Ahora: {usuarioActualizado.Credito:C}";

            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> PagarLiquidacionAsync()
        {
            var credito = await _servicesApi.ObtenerCreditoLiquidacionAsync();
            ViewBag.CreditoLiquidacion = credito;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> PagarLiquidacion(PagoLiquidacionDTO dto)
        {

            var resultado = await _servicesApi.PagarLiquidacionAsync(dto);
            var credito = await _servicesApi.ObtenerCreditoLiquidacionAsync();

            ViewBag.Mensaje = resultado;
            ViewBag.CreditoLiquidacion = credito;

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

        // Agrega estos métodos a tu UsuariosController
        [HttpGet]
        public async Task<IActionResult> VerificarDatos(string numeroControl, string correo)
        {
            var response = new
            {
                numeroControlExiste = !string.IsNullOrEmpty(numeroControl)
                    ? await _servicesApi.VerificarNumeroControlExistenteAsync(numeroControl)
                    : false,
                correoExiste = !string.IsNullOrEmpty(correo)
                    ? await _servicesApi.VerificarCorreoExistenteAsync(correo)
                    : false,
                // Obtener datos locales de la tabla (cache)
                numerosControlTabla = (await _servicesApi.GetUsuariosAsync()).Select(u => u.NumeroControl).ToList(),
                correosTabla = (await _servicesApi.GetUsuariosAsync()).Select(u => u.Correo).ToList()
            };

            return Json(response);
        }


    }
}