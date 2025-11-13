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
                var (token, rol, numeroControl) = await _services.LoginAsync(login);

                // 🔹 ESTA LÍNEA ERA LA QUE FALTABA (muy importante)
                if (!string.IsNullOrEmpty(numeroControl))
                {
                    HttpContext.Session.SetString("NumeroControl", numeroControl);
                }

                // Crear claims
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, login.Correo),
                    new Claim(ClaimTypes.Role, rol),
                    new Claim("JwtToken", token)
                };

                if (rol == "alumno" && !string.IsNullOrEmpty(numeroControl))
                {
                    claims.Add(new Claim("NumeroControl", numeroControl));
                }

                claims.Add(new Claim("FirstLogin", "true"));


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
                    authProperties);

                // Redirigir según rol
                return rol switch
                {
                    "ventas" => RedirectToAction("Index", "Ventas"),
                    "inventario" => RedirectToAction("HomeInventario", "Login"),
                    "alumno" => RedirectToAction("MiCuenta", "Usuarios"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en login: {ex.Message}");
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
