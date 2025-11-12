using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontCafeteriaMVC.Models
{
    public class Usuario
    {
        [Key]
        [Column("id_usuario")]
        public int IdUsuario { get; set; }

        [Column("nombre_usuario")]
        public string NombreUsuario { get; set; } = null!;

        [Column("correo_usuario")]
        public string CorreoUsuario { get; set; } = null!;

        [Column("numero_control")]
        public string? NumeroControl { get; set; }

        [Column("credito")]
        public decimal Credito { get; set; }

        [Column("huella")]
        public string? Huella { get; set; }

        [Column("rol_usuario")]
        public string? RolUsuario { get; set; }

        [Column("codigqrtexto")]
        public string? CodigoQRTexto { get; set; }

        [Column("contra_usuario")]
        public string ContraUsuario { get; set; } = null!;
    }
}
