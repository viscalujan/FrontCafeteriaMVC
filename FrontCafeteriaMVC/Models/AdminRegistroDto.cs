using System.ComponentModel.DataAnnotations;

namespace FrontCafeteriaMVC.Models
{
    public class AdminRegistroDTO
    {
        [Required]
        [Display(Name = "Nombre")]
        public string NombreAut { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Correo")]
        public string CorreoAut { get; set; } = null!;

        [Required]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [Display(Name = "Contraseña")]
        public string ContraAut { get; set; } = null!;

        [Required]
        [Display(Name = "Rol")]
        public string RolAut { get; set; } = null!;
    }
}
