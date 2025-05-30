using FrontCafeteriaMVC.Filters;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    [AuthorizeSession]
    public class ProductosController : Controller
    {
        private readonly IServicesAPI _servicesApi;

        public ProductosController(IServicesAPI servicesApi)
        {
            _servicesApi = servicesApi;
        }

        public async Task<IActionResult> Index()
        {
            var productos = await _servicesApi.GetProductosAsync();
            return View(productos);
        }

        public async Task<IActionResult> Crear()
        {
            return View("AgregarEditar", new Producto());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Producto producto)
        {
            if (!ModelState.IsValid)
                return View("AgregarEditar", producto);

            var resultado = await _servicesApi.CrearProductoAsync(producto);
            if (resultado)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "No se pudo crear el producto.");
            return View("AgregarEditar", producto);
        }

        public async Task<IActionResult> Editar(int id)
        {
            var producto = await _servicesApi.GetProductoByIdAsync(id);
            if (producto == null)
                return NotFound();

            return View("AgregarEditar", producto);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Producto producto)
        {
            if (!ModelState.IsValid)
                return View("AgregarEditar", producto);

            var resultado = await _servicesApi.ActualizarProductoAsync(producto);
            if (resultado)
                return RedirectToAction(nameof(Index));

            ModelState.AddModelError(string.Empty, "No se pudo actualizar el producto.");
            return View("AgregarEditar", producto);
        }
    }
}
