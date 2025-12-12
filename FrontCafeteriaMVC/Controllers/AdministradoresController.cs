using FrontCafeteriaMVC.Filters;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    [AuthorizeSession]
    [AuthorizeRole("admin")]
    public class AdministradoresController : Controller
    {
        private readonly IServicesAPI _api;

        public AdministradoresController(IServicesAPI api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Crear()
        {
            // El filtro AuthorizeRole ya garantizó que sea admin
            return View(new AdminRegistroDTO());
        }

        [HttpPost]
        public async Task<IActionResult> Crear(AdminRegistroDTO model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ok = await _api.RegistrarAdminAsync(model);

            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar el usuario administrador.");
                return View(model);
            }

            TempData["Exito"] = "Usuario administrador creado correctamente.";
            return RedirectToAction("Crear");
        }
    }
}
