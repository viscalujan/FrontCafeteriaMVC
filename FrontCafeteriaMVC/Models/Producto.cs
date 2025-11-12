using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class Producto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0.")]
        [JsonPropertyName("precio")]
        public decimal Precio { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa.")]
        [JsonPropertyName("cantidadProducto")]
        public int Cantidad { get; set; }

        // Alias de compatibilidad si el backend expone estos nombres:
        [JsonPropertyName("nombreProducto")]
        public string? NombreProductoAlias
        {
            get => Nombre;
            set { if (!string.IsNullOrWhiteSpace(value)) Nombre = value!; }
        }
    }
}
