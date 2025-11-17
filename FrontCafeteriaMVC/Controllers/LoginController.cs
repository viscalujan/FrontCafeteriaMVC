using System.Security.Claims;
using FrontCafeteriaMVC.Models;
using FrontCafeteriaMVC.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace FrontCafeteriaMVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IServicesAPI _services;

        public LoginController(IServicesAPI services)
        {
            _services = services;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequest login)
        {
            try
            {
                // 🔥 Llamamos al servicio (ya devuelve LoginResponse)
                var resp = await _services.LoginAsync(login);

                // ======================================================
                // 🔹 1) PRIMER INICIO DE SESIÓN
                // ======================================================
                if (resp.requiereCambio == true && resp.modo == "primerInicio")
                {
                    TempData["Correo"] = login.Correo;
                    TempData["ToastType"] = "warning";
                    TempData["ToastMessage"] = resp.mensaje;

                    return RedirectToAction("Index", "PrimerInicio");
                }

                // ======================================================
                // 🔹 2) LOGIN NORMAL
                // ======================================================
                if (string.IsNullOrEmpty(resp.token))
                {
                    ViewBag.Error = "Credenciales inválidas.";
                    return View(login);
                }

                // Guardar token y claims
                HttpContext.Session.SetString("token", resp.token);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.Correo),
                    new Claim(ClaimTypes.Role, resp.rol),
                    new Claim("JwtToken", resp.token)
                };

                if (!string.IsNullOrEmpty(resp.numeroControl))
                {
                    claims.Add(new Claim("NumeroControl", resp.numeroControl));
                    HttpContext.Session.SetString("NumeroControl", resp.numeroControl);
                }

                var authProperties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    claimsPrincipal,
                    authProperties
                );

                // ======================================================
                // 🔹 REDIRECCIÓN SEGÚN ROL
                // ======================================================
                return resp.rol switch
                {
                    "ventas" => RedirectToAction("Index", "Ventas"),
                    "inventario" => RedirectToAction("HomeInventario", "Login"),
                    "alumno" => RedirectToAction("MiCuenta", "Usuarios"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Login: {ex.Message}");
                ViewBag.Error = "Credenciales inválidas.";
                return View(login);
            }
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }

        public IActionResult HomeInventario()
        {
            return View();
        }
    }
}
