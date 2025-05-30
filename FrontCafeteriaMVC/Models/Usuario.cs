using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe ser un correo válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El número de control es obligatorio.")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de control debe ser de 8 dígitos.")]
        [JsonPropertyName("Contra")] // 👈 Aquí mapeas que viene/va como "Contra"
        public string NumeroControl { get; set; }

        public string Huella { get; set; } // En futuro para huella

        [Range(50, double.MaxValue, ErrorMessage = "El crédito inicial debe ser mayor o igual a 50.")]
        public decimal Credito { get; set; }

        public int Rol { get; set; } = 0;
    }
}