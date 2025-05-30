using FrontCafeteriaMVC.Filters;
using FrontCafeteriaMVC.Helpers;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Net.Http;
using System.Security.Policy;

namespace FrontCafeteriaMVC.Controllers
{
    [AuthorizeSession]
    public class VentasController : Controller
    {
        private readonly IServicesAPI _api;
        private readonly IHttpContextAccessor _http;

        public VentasController(IServicesAPI api, IHttpContextAccessor http)
        {
            _api = api;
            _http = http;
        }

        public async Task<IActionResult> Index(string? buscar)
        {
            var productos = await _api.GetProductosAsync();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                buscar = buscar.Trim().ToLower();
                productos = productos
                    .Where(p => p.Nombre != null && p.Nombre.ToLower().Contains(buscar))
                    .ToList();
            }

            var carrito = _http.HttpContext!.Session.GetObjectFromJson<List<DetalleVenta>>("carrito") ?? new();
            var productosDic = productos.ToDictionary(p => p.Id, p => p);

            // Calcular total correctamente
            decimal total = 0;
            foreach (var item in carrito)
            {
                if (productosDic.TryGetValue(item.ProductoId, out var producto))
                {
                    total += item.Cantidad * producto.Precio;
                }
            }

            Ticket? ticket = null;
            if (TempData["TicketId"] != null && TempData["TicketTotal"] != null)
            {
                // Parsear correctamente el valor monetario
                var totalString = TempData["TicketTotal"]!.ToString()!;
                if (decimal.TryParse(totalString, NumberStyles.Currency, CultureInfo.CurrentCulture, out var totalDecimal))
                {
                    ticket = new Ticket
                    {
                        Id = (int)TempData["TicketId"]!,
                        Total = totalDecimal,
                        FechaVenta = DateTime.Now
                    };
                }
            }

            var model = new VentasViewModel
            {
                Productos = productos,
                Carrito = carrito,
                ProductosDic = productosDic,
                Ticket = ticket,
                Buscar = buscar,
                Total = total // Agregamos el total calculado al modelo
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int ProductoId, int Cantidad)
        {
            var producto = await _api.GetProductoByIdAsync(ProductoId);
            if (producto == null || Cantidad <= 0 || Cantidad > producto.Cantidad)
                return BadRequest("Producto no válido o cantidad excede el stock.");

            var carrito = _http.HttpContext!.Session.GetObjectFromJson<List<DetalleVenta>>("carrito") ?? new();

            var existente = carrito.FirstOrDefault(c => c.ProductoId == ProductoId);
            if (existente != null)
            {
                existente.Cantidad += Cantidad;
            }
            else
            {
                carrito.Add(new DetalleVenta
                {
                    ProductoId = ProductoId,
                    Cantidad = Cantidad
                });
            }

            _http.HttpContext.Session.SetObjectAsJson("carrito", carrito);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult QuitarDelCarrito(int ProductoId)
        {
            var carrito = _http.HttpContext!.Session.GetObjectFromJson<List<DetalleVenta>>("carrito") ?? new();
            var item = carrito.FirstOrDefault(c => c.ProductoId == ProductoId);
            if (item != null)
                carrito.Remove(item);

            _http.HttpContext.Session.SetObjectAsJson("carrito", carrito);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarVenta(string metodoPago, string hashQR)
        {
            try
            {
                var carrito = _http.HttpContext.Session.GetObjectFromJson<List<DetalleVenta>>("carrito");
                if (carrito == null || !carrito.Any())
                {
                    TempData["Error"] = "El carrito está vacío";
                    return RedirectToAction("Index");
                }

                var venta = new VentaCreate
                {
                    MetodoPago = metodoPago?.ToLower() ?? "",
                    HashQR = hashQR,
                    Detalles = carrito
                };

                if (venta.MetodoPago == "credito" && string.IsNullOrWhiteSpace(venta.HashQR))
                {
                    TempData["Error"] = "Debe escanear el código QR para pagos con crédito";
                    return RedirectToAction("Index");
                }

                var ticket = await _api.CrearVentaConTicketAsync(venta);
                if (ticket == null)
                {
                    TempData["Error"] = "No se pudo procesar la venta";
                    return RedirectToAction("Index");
                }

                _http.HttpContext.Session.Remove("carrito");
                TempData["Success"] = $"Venta realizada! Ticket #{ticket.Id}";
                TempData["TicketId"] = ticket.Id;
                TempData["TicketTotal"] = ticket.Total.ToString("F2"); // Cambiado a formato numérico
                TempData["TicketMetodo"] = ticket.Metodo;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Reporte(DateTime? desde, DateTime? hasta)
        {
            // Valores por defecto: últimos 30 días
            hasta ??= DateTime.Today;
            desde ??= hasta.Value.AddDays(-30);

            try
            {
                var ventasAgrupadas = await _api.GetVentasAgrupadasAsync(desde, hasta);

                var modelo = new ReporteVentasViewModel
                {
                    Desde = desde,
                    Hasta = hasta,
                    VentasAgrupadas = ventasAgrupadas
                };

                return View(modelo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al obtener reporte: {ex.Message}";
                return View(new ReporteVentasViewModel());
            }
        }

        [HttpGet("GenerarExcel")]
        public async Task<IActionResult> GenerarExcel(DateTime? desde, DateTime? hasta)
        {
            try
            {
                var excelBytes = await _api.GenerarExcelReporteAsync(desde, hasta);
                return File(excelBytes,
                          "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                          $"ReporteVentas_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar Excel: {ex.Message}";
                return RedirectToAction("Reporte");
            }
        }

    }

}