using FrontCafeteriaMVC.Services;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Razor;

var builder = WebApplication.CreateBuilder(args);

// Configurar la base URL del backend
var baseurl = builder.Configuration["AppiSettings:BaseURL"];

// MVC con vistas
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    })
    .AddRazorRuntimeCompilation(); // Refleja cambios en vistas sin recompilar

// Cliente HTTP para consumir la API
builder.Services.AddHttpClient<IServicesAPI, ServicesAPI>(client =>
{
    client.BaseAddress = new Uri(baseurl);
});

// Permite acceso a HttpContext
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Configura sesión
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // más tiempo para pruebas
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configura autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Index";
        options.AccessDeniedPath = "/Login/AccesoDenegado";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();           // Primero sesión
app.UseAuthentication();    // Luego autenticación
app.UseAuthorization();     // Luego autorización

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();