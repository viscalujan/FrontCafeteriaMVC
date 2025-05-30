using FrontCafeteriaMVC.Models;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace FrontCafeteriaMVC.Services
{
    public class ServicesAPI : IServicesAPI
    {
        private readonly HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ServicesAPI(HttpClient http, IHttpContextAccessor httpContextAccessor)
        {
            _http = http;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string token, string rol, string? numeroControl)> LoginAsync(LoginRequest login)
        {
        
            var response = await _http.PostAsJsonAsync("api/Auth/login", login);

            if (!response.IsSuccessStatusCode)
                throw new Exception("Credenciales inválidas");

            // Lee la respuesta genérica (puede o no tener numeroControl)
            var result = await response.Content.ReadFromJsonAsync<FrontCafeteriaMVC.Models.LoginResponse>();

            // Guarda el token en sesión y lo configura para futuras peticiones
            _httpContextAccessor.HttpContext!.Session.SetString("token", result.token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.token);

            // Si el rol NO es alumno, numeroControl será null
            return (result.token, result.rol, result.numeroControl);
        }
        public async Task<List<Producto>> GetProductosAsync()
        {
            AgregarTokenHeader();
            var response = await _http.GetAsync("api/Productos");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<Producto>>(json);
        }

        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            AgregarTokenHeader();
            var response = await _http.GetAsync($"api/Productos/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Producto>();
        }

        public async Task<bool> CrearProductoAsync(Producto producto)
        {
            AgregarTokenHeader();
            var response = await _http.PostAsJsonAsync("api/Productos", producto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ActualizarProductoAsync(Producto producto)
        {
            AgregarTokenHeader();
            var response = await _http.PutAsJsonAsync($"api/Productos/{producto.Id}", producto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> CrearVentaAsync(VentaCreate venta)
        {
            AgregarTokenHeader();
            var response = await _http.PostAsJsonAsync("api/Ventas", venta);
            return response.IsSuccessStatusCode;
        }

        public async Task<Ticket> CrearVentaConTicketAsync(VentaCreate venta)
        {
            AgregarTokenHeader();
            var response = await _http.PostAsJsonAsync("api/Ventas", venta);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error al crear venta: {response.StatusCode} - {error}");
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Ticket>(json);
        }

        public async Task<List<UsuarioDTO>> GetUsuariosAsync()
        {
            AgregarTokenHeader();
            var response = await _http.GetAsync("api/Usuarios");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<UsuarioDTO>>(json);
        }

        public async Task<bool> RegistrarUsuarioAsync(UsuarioRegistroDTO usuario)
        {
            AgregarTokenHeader();
            var response = await _http.PostAsJsonAsync("api/Usuarios", usuario);
            return response.IsSuccessStatusCode;
        }

        public async Task<Usuario> GetUsuarioPorNumeroControlAsync(string numeroControl)
        {
            AgregarTokenHeader();
            var response = await _http.GetAsync($"api/Usuarios/numeroControl/{numeroControl}");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Usuario>(json);
        }

        public async Task<bool> ActualizarCreditoUsuarioAsync(int id, decimal nuevoCredito)
        {
            AgregarTokenHeader();
            var data = new { Credito = nuevoCredito };
            var response = await _http.PutAsJsonAsync($"api/Usuarios/{id}/credito", data);
            return response.IsSuccessStatusCode;
        }

        private void AgregarTokenHeader()
        {
            var token = _httpContextAccessor.HttpContext!.Session.GetString("token");
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        private class LoginResponse
        {

            public string token { get; set; }
            public string rol { get; set; }
        }

        public async Task<Ticket?> GenerarVentaAsync(VentaCreate venta, string token)
        {
            AgregarTokenHeader();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var json = System.Text.Json.JsonSerializer.Serialize(venta);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://localhost:7123/api/Ventas", content);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();
                var ticket = await System.Text.Json.JsonSerializer.DeserializeAsync<Ticket>(responseStream);
                return ticket;
            }

            return null;
        }

        public async Task<bool> AumentarCreditoAsync(AumentoCreditoDTO dto)
        {
            AgregarTokenHeader();
            var response = await _http.PostAsJsonAsync("api/usuarios/aumentar-credito", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<HistorialCredito>> ObtenerHistorialCreditoGeneralAsync()
        {
            AgregarTokenHeader();
            string url = "api/Usuarios/historial-credito";
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<HistorialCredito>>();
            }

            return new List<HistorialCredito>();
        }

        public async Task<List<HistorialCredito>> ObtenerHistorialCreditoAsync(string numeroControl)
        {
            AgregarTokenHeader();
            if (string.IsNullOrWhiteSpace(numeroControl))
                return await ObtenerHistorialCreditoGeneralAsync(); // Cambio importante aquí

            string url = $"api/Usuarios/historial-credito/{numeroControl}";
            var response = await _http.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<HistorialCredito>>();
            }

            return new List<HistorialCredito>();
        }


        public async Task<List<Venta>> GetVentasFiltradasAsync(DateTime? desde, DateTime? hasta)
        {
            AgregarTokenHeader();

            var token = _httpContextAccessor.HttpContext?.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
                return new List<Venta>(); // O podrías redirigir al login

            // Agrega el token a la cabecera
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = "api/reportes/ventas";
            if (desde.HasValue || hasta.HasValue)
            {
                url += $"?desde={desde:yyyy-MM-dd}&hasta={hasta:yyyy-MM-dd}";
            }

            var response = await _http.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Venta>>() ?? new List<Venta>();
            }

            return new List<Venta>();
        }
        public async Task<List<DetalleVentaView>> GetReporteDetalladoAsync(DateTime? desde, DateTime? hasta)
        {
            // 1. Agregar el token de autenticación
            AgregarTokenHeader(); // <- ¡Esto faltaba!

            // 2. Formatear fechas correctamente
            var desdeStr = desde?.ToString("yyyy-MM-dd");
            var hastaStr = hasta?.ToString("yyyy-MM-dd");

            // 3. Realizar la petición
            var response = await _http.GetAsync(
                $"api/ventas/reporte-detallado?desde={desdeStr}&hasta={hastaStr}");

            // 4. Manejo de errores mejorado
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error en API: {response.StatusCode} - {errorContent}");

                // Si es un error 401 (No autorizado), podrías redirigir al login
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Sesión expirada. Por favor vuelva a iniciar sesión.");
                }

                return new List<DetalleVentaView>();
            }

            // 5. Deserializar con opciones para case insensitive
            return await response.Content.ReadFromJsonAsync<List<DetalleVentaView>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<DetalleVentaView>();
        }


        public async Task<List<VentaAgrupada>> GetVentasAgrupadasAsync(DateTime? desde, DateTime? hasta)
        {

            AgregarTokenHeader();
            var detalles = await GetReporteDetalladoAsync(desde, hasta);

            return detalles
                .GroupBy(d => d.VentaId)
                .Select(g => new VentaAgrupada
                {
                    VentaId = g.Key,
                    Fecha = g.First().Fecha,
                    MetodoPago = g.First().MetodoPago,
                    Detalles = g.ToList()
                })
                .OrderByDescending(v => v.Fecha)
                .ToList();
        }


        public async Task<byte[]> GenerarExcelReporteAsync(DateTime? desde, DateTime? hasta)
        {
            AgregarTokenHeader();
            // Configuración de licencia EPPlus (requerido desde v8+)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            // Obtener datos agrupados
            var ventasAgrupadas = await GetVentasAgrupadasAsync(desde, hasta);

            using (var excelPackage = new ExcelPackage())
            {
                // Crear hoja de trabajo
                var worksheet = excelPackage.Workbook.Worksheets.Add("Reporte_Ventas");

                // ===== CONFIGURACIÓN DE ESTILOS =====
                var headerStyle = worksheet.Cells["A1:G1"].Style;
                headerStyle.Fill.PatternType = ExcelFillStyle.Solid;
                headerStyle.Fill.BackgroundColor.SetColor(Color.LightBlue);
                headerStyle.Font.Bold = true;
                headerStyle.Border.BorderAround(ExcelBorderStyle.Thin);
                headerStyle.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                var moneyStyle = excelPackage.Workbook.Styles.CreateNamedStyle("Money");
                moneyStyle.Style.Numberformat.Format = "$#,##0.00";

                // ===== CABECERA DEL REPORTE =====
                worksheet.Cells["A1"].Value = "REPORTE DE VENTAS - CAFETERÍA";
                worksheet.Cells["A1:G1"].Merge = true;

                worksheet.Cells["A2"].Value = "Período:";
                worksheet.Cells["B2"].Value = $"{desde?.ToString("dd/MM/yyyy")} - {hasta?.ToString("dd/MM/yyyy")}";
                worksheet.Cells["A2:B2"].Style.Font.Bold = true;

                // ===== ENCABEZADOS DE COLUMNAS =====
                int currentRow = 4;
                worksheet.Cells[currentRow, 1].Value = "Ticket #";
                worksheet.Cells[currentRow, 2].Value = "Fecha";
                worksheet.Cells[currentRow, 3].Value = "Producto";
                worksheet.Cells[currentRow, 4].Value = "Cantidad";
                worksheet.Cells[currentRow, 5].Value = "P. Unitario";
                worksheet.Cells[currentRow, 6].Value = "Subtotal";
                worksheet.Cells[currentRow, 7].Value = "Método Pago";

                // Aplicar estilo a los encabezados
                worksheet.Cells[currentRow, 1, currentRow, 7].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 1, currentRow, 7].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                worksheet.Cells[currentRow, 1, currentRow, 7].Style.Font.Bold = true;

                currentRow++;

                // ===== DATOS DE VENTAS =====
                foreach (var venta in ventasAgrupadas)
                {
                    foreach (var detalle in venta.Detalles)
                    {
                        worksheet.Cells[currentRow, 1].Value = venta.VentaId;
                        worksheet.Cells[currentRow, 2].Value = venta.Fecha;
                        worksheet.Cells[currentRow, 2].Style.Numberformat.Format = "dd/MM/yyyy HH:mm";
                        worksheet.Cells[currentRow, 3].Value = detalle.ProductoNombre;
                        worksheet.Cells[currentRow, 4].Value = detalle.Cantidad;
                        worksheet.Cells[currentRow, 5].Value = detalle.PrecioUnitario;
                        worksheet.Cells[currentRow, 5].StyleName = "Money";
                        worksheet.Cells[currentRow, 6].Value = detalle.Total;
                        worksheet.Cells[currentRow, 6].StyleName = "Money";
                        worksheet.Cells[currentRow, 7].Value = venta.MetodoPago;

                        currentRow++;
                    }

                    // ===== TOTAL POR TICKET =====
                    worksheet.Cells[currentRow, 5].Value = "Total Ticket:";
                    worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
                    worksheet.Cells[currentRow, 6].Value = venta.TotalVenta;
                    worksheet.Cells[currentRow, 6].StyleName = "Money";
                    worksheet.Cells[currentRow, 6].Style.Font.Bold = true;

                    // Añadir borde inferior
                    worksheet.Cells[currentRow, 1, currentRow, 7].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    currentRow += 2;
                }

                // ===== TOTAL GENERAL =====
                worksheet.Cells[currentRow, 5].Value = "TOTAL GENERAL:";
                worksheet.Cells[currentRow, 5].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 6].Value = ventasAgrupadas.Sum(v => v.TotalVenta);
                worksheet.Cells[currentRow, 6].StyleName = "Money";
                worksheet.Cells[currentRow, 6].Style.Font.Bold = true;
                worksheet.Cells[currentRow, 6].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[currentRow, 6].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);

                // ===== AJUSTES FINALES =====
                // Autoajustar columnas
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                // Congelar paneles (fijar encabezados)
                worksheet.View.FreezePanes(5, 1);

                return excelPackage.GetAsByteArray();
            }
        }

        public async Task<string> PagarLiquidacionAsync(PagoLiquidacionDTO dto)
        {
            AgregarTokenHeader(); // si aplica

            var response = await _http.PostAsJsonAsync("api/Usuarios/pagar-liquidacion", dto);

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return $"✅ {json}";
            }

            var error = await response.Content.ReadAsStringAsync();
            return $"❌ Error: {response.StatusCode} - {error}";
        }

        public async Task<decimal?> ObtenerCreditoAsync(string numeroControl)
        {
            AgregarTokenHeader();
            var resp = await _http.GetAsync($"api/UsuarioNC/credito/{numeroControl}");
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("credito").GetDecimal();
        }

        public async Task<List<HistorialCreditoVM>> ObtenerHistorialAsync(string numeroControl)
        {
            AgregarTokenHeader();
            var resp = await _http.GetAsync($"api/UsuarioNC/historial-credito/{numeroControl}");
            if (!resp.IsSuccessStatusCode) return new();

            return await resp.Content.ReadFromJsonAsync<List<HistorialCreditoVM>>();
        }

        public async Task<(string base64, string downloadUrl)> ObtenerQrAsync(string numeroControl)
        {
            AgregarTokenHeader();
            var resp = await _http.GetAsync($"api/UsuarioNC/qr/{numeroControl}");
            if (!resp.IsSuccessStatusCode) return (null, null);

            var bytes = await resp.Content.ReadAsByteArrayAsync();
            var base64 = Convert.ToBase64String(bytes);
            // misma URL sirve para descargar
            var downloadUrl = _http.BaseAddress + $"api/UsuarioNC/qr/{numeroControl}";
            return ($"data:image/png;base64,{base64}", downloadUrl);
        }



    }
}