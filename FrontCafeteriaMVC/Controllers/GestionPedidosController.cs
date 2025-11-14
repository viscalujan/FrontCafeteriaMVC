using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    public class GestionPedidosController : Controller
    {
        private readonly IServicesAPI _api;

        public GestionPedidosController(IServicesAPI api)
        {
            _api = api;
        }

        public async Task<IActionResult> Index()
        {
            var pedidos = await _api.GetPedidosAsync();
            return View(pedidos);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CambiarEstado(int idPedido, int nuevoEstado)
        {
            await _api.CambiarEstadoPedidoAsync(idPedido, nuevoEstado);
            return RedirectToAction("Index");
        }

    }


}
