using System.ComponentModel.DataAnnotations;

namespace FrontCafeteriaMVC.Models
{
    public class UsuarioRegistroDTO
    {
        [Required]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚáéíóúÑñ]+\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+\s[A-Za-zÁÉÍÓÚáéíóúÑñ]+$", ErrorMessage = "Debe ingresar al menos nombre y dos apellidos.")]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido.")]
        public string Correo { get; set; }

        [Required]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "El número de control debe tener exactamente 8 dígitos.")]
        public string NumeroControl { get; set; }

        [Required]
        [Range(50, double.MaxValue, ErrorMessage = "El crédito inicial debe ser al menos 50.")]
        public decimal Credito { get; set; }

        [Required]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string Contrasena { get; set; }

    }
}
