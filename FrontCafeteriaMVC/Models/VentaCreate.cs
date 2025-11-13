using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class VentaCreate
    {
        // 🔹 Debe coincidir con "FkIdUsuario" del backend
        [JsonPropertyName("fkIdUsuario")]
        public int FkIdUsuario { get; set; }

        // 🔹 Método de pago: "efectivo" o "credito"
        [JsonPropertyName("metodoPago")]
        public string MetodoPago { get; set; } = "efectivo";

        // 🔹 Opcional: número de control del usuario (para crédito)
        [JsonPropertyName("numeroDeControl")]
        public string? NumeroDeControl { get; set; }

        // 🔹 Opcional: hash del QR (para crédito)
        [JsonPropertyName("hashQR")]
        public string? HashQR { get; set; }

        // 🔹 Fecha de la venta (el back también la acepta)
        [JsonPropertyName("fechaVenta")]
        public DateTime FechaVenta { get; set; } = DateTime.Now;

        // 🔹 Detalles de la venta (lista de productos y cantidades)
        [JsonPropertyName("detalles")]
        public List<VentaDetalle> Detalles { get; set; } = new();

    }
}
