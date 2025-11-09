using System.ComponentModel.DataAnnotations;

namespace FrontCafeteriaMVC.Models
{
    public class UsuarioCreateDTO
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required, EmailAddress] public string Correo { get; set; } = string.Empty;
        [Required, RegularExpression(@"^\d{8}$")] public string NumeroControl { get; set; } = string.Empty;
        [Range(0, double.MaxValue)] public decimal Credito { get; set; }
        [MinLength(8)] public string Contrasena { get; set; } = string.Empty;
        public string? RolUsuario { get; set; }  // opcional si el endpoint lo usa
    }
}
