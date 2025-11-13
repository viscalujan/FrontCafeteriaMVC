using FrontCafeteriaMVC.Filters;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FrontCafeteriaMVC.Controllers
{
    [AuthorizeSession]
    //[Authorize(Roles = "alumno")]

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

        // [HttpPost]
        //public async Task<IActionResult> Crear(UsuarioRegistroDTO usuarioRegistroDTO)
        // {
        //   if (!ModelState.IsValid)
        //      return View(usuarioRegistroDTO);

        //    var resultado = await _servicesApi.RegistrarUsuarioAsync(usuarioRegistroDTO);

        //   if (resultado)
        //           return RedirectToAction(nameof(Index));

        //    ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario.");
        //   return View(usuarioRegistroDTO);
        //   }
        [HttpPost]
        public async Task<IActionResult> Crear(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            if (!ModelState.IsValid)
                return View(usuarioRegistroDTO);

            var resultado = await _servicesApi.RegistrarUsuarioAsync(usuarioRegistroDTO);

            if (!resultado)
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario.");
                return View(usuarioRegistroDTO);
            }

            // 🔹 GENERAR Y ENVIAR EL QR AUTOMÁTICAMENTE
            var qrOk = await _servicesApi.EnviarQRAsync(usuarioRegistroDTO.Correo, usuarioRegistroDTO.NumeroControl);

            if (!qrOk)
            {
                TempData["Error"] = "El usuario fue creado, pero no se pudo generar ni enviar su QR.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Exito"] = "Usuario creado correctamente y QR enviado.";
            return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> HistorialCredito(DateTime? desde, DateTime? hasta, string? numeroControl)
        {
            // 1. Si el usuario invierte fechas, corregimos automáticamente
            if (desde.HasValue && hasta.HasValue && desde > hasta)
            {
                var temp = desde;
                desde = hasta;
                hasta = temp;
            }

            // 2. Obtener datos del API
            var historial = await _servicesApi.ObtenerHistorialFiltradoAsync(desde, hasta, numeroControl);

            // 3. Preparar modelo para la vista
            var model = new HistorialCreditoFiltroViewModel
            {
                Historial = historial,
                NumeroControl = numeroControl,
                FechaInicio = desde,
                FechaFin = hasta
            };

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> ExportarHistorial(
            string? numeroControl,
            DateTime? desde,
            DateTime? hasta)
        {
            var bytes = await _servicesApi.ExportarHistorialExcelAsync(desde, hasta, numeroControl);

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"HistorialCredito_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            );
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

        // public async Task<IActionResult> MiCuenta()
        // {
        //    var numeroControl = User.Claims.FirstOrDefault(c => c.Type == "NumeroControl")?.Value;
        //   var nombre = User.Identity?.Name;

        //  if (string.IsNullOrWhiteSpace(numeroControl))
        //      return RedirectToAction("Index", "Login");

        //   var credito = await _servicesApi.ObtenerCreditoAsync(numeroControl) ?? 0;
        //  var historial = await _servicesApi.ObtenerHistorialCreditoAsync(numeroControl);

        //  var (base64, url) = await _servicesApi.ObtenerQrAsync(numeroControl);

        //  var vm = new MiCuentaVM
        //  {
        //  NumeroControl = numeroControl,
        //    Nombre = nombre,
        //   Credito = credito,
        //  Historial = historial.Select(h => new HistorialCreditoVM
        //  {
        //Fecha = h.Fecha,
        //  Cantidad = h.Cantidad,
        //    AutCorreo = h.AutCorreo
        //  }).ToList(),
        //      QrBase64 = base64,             // 👉 sin el prefijo aquí
        //    QrDownloadUrl = url
        //   };

        //  return View(vm);
        //}
        public async Task<IActionResult> MiCuenta()

        {

            // 🔹 Manejo del flag para quitar el botón "Regresar" en primer acceso
            var identity = (ClaimsIdentity)User.Identity;
            var loginClaim = identity.FindFirst("FirstLogin");

            if (loginClaim != null && loginClaim.Value == "true")
            {
                identity.RemoveClaim(loginClaim);
                identity.AddClaim(new Claim("FirstLogin", "false"));

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity)
                );
            }



            // Ahora sí toma el número de control CORRECTO
            var numeroControl = HttpContext.Session.GetString("NumeroControl");
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
                QrBase64 = base64,
                QrDownloadUrl = url
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ActualizarQR(string numeroControl)
        {
            try
            {
                var (base64, downloadUrl) = await _servicesApi.RegenerarQRAsync(numeroControl);

                if (base64 == null)
                    return Json(new { success = false, message = "No se pudo regenerar el QR" });

                return Json(new
                {
                    success = true,
                    qrBase64 = base64,
                    downloadUrl = downloadUrl
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }



    }
}
