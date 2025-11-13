using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrontCafeteriaMVC.Controllers
{
    public class PedidosController : Controller
    {
        private readonly IServicesAPI _api;

        public PedidosController(IServicesAPI api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Obtener NumeroControl desde sesión
            var numeroControl = HttpContext.Session.GetString("NumeroControl");

            Console.WriteLine(">>> NumeroControl Session = " + numeroControl);

            if (string.IsNullOrEmpty(numeroControl))
            {
                TempData["Error"] = "Tu sesión ha expirado. Inicia sesión nuevamente.";
                return RedirectToAction("Login", "Auth");
            }

            // 2. Obtener usuario completo desde el API
            var usuario = await _api.GetUsuarioPorNumeroControlAsync(numeroControl);

            if (usuario == null)
            {
                TempData["Error"] = "No se pudo cargar tu información.";
                return RedirectToAction("Login", "Auth");
            }

            // 🔥 3. ENVIAR IdUsuario AL FRONT (esto es lo importante)
            ViewBag.UsuarioId = usuario.Id;

            // 4. Cargar productos
            var productos = await _api.GetProductosAsync();

            return View(productos);
        }


        [HttpPost]
        public async Task<IActionResult> RealizarPedido([FromBody] PedidoCreateDTO pedido)
        {
            try
            {
                Console.WriteLine("🔥 LLEGÓ PETICIÓN RealizarPedido");

                if (pedido == null)
                {
                    Console.WriteLine("❌ ERROR: Pedido es NULL");
                    return StatusCode(500, new { error = "pedido es null" });
                }

                Console.WriteLine("UsuarioId = " + pedido.UsuarioId);
                Console.WriteLine("Detalles count = " + (pedido.Detalles?.Count ?? 0));

                var respuestaApi = await _api.CrearPedidoAsync(pedido);

                Console.WriteLine("🔥 RESPUESTA DEL API:");
                Console.WriteLine(respuestaApi);

                return Ok(JsonConvert.DeserializeObject(respuestaApi));
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥🔥 ERROR REAL EN MVC:");
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new { error = ex.Message, detalle = ex.ToString() });
            }
        }







    }

}
