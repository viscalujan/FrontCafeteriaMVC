using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class VentaCreate
    {
        public string MetodoPago { get; set; } = string.Empty;

        public int? UsuarioId { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();

        [JsonPropertyName("numeroDeControl")]
        public string? NumeroDeControl { get; set; }

        public string HashQR { get; set; }

    }
}
