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

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Correo),
            new Claim(ClaimTypes.Role, rol),
            new Claim("JwtToken", token) // Guardamos el token JWT como claim
        };

                if (rol == "alumno" && !string.IsNullOrEmpty(numeroControl))
                {
                    claims.Add(new Claim("NumeroControl", numeroControl));
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
                    authProperties);

                // Redirige según el rol
                return rol switch
                {
                    "ventas" => RedirectToAction("index", "Ventas"),
                    "inventario" => RedirectToAction("HomeInventario", "Login"),
                    "alumno" => RedirectToAction("MiCuenta", "Usuarios"),
                    _ => RedirectToAction("Index", "Home")
                };
            }
            catch (Exception ex)
            {
                // Log del error para diagnóstico
                Console.WriteLine($"Error en login: {ex.Message}");
                ViewBag.Error = "Credenciales inválidas.";
                return View(login);
            }
        }

        [HttpPost]
        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); 
            return RedirectToAction("Index", "Login"); 
        }


        public IActionResult HomeInventario()
        {
            return View();
        }
    }
}
