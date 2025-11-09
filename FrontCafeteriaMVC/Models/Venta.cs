using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontCafeteriaMVC.Models
{
    public class Venta
    {
        [Key]
        [Column("id_ventas")]
        public int IdVentas { get; set; }

        [Column("FK_id_usuario")]
        public int FkIdUsuario { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; }

        [Column("total_venta")]
        public decimal TotalVenta { get; set; }

        [Column("metodo_pago")]
        public string MetodoPago { get; set; } = null!;

        public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();

        [NotMapped]
        public decimal Total { get => TotalVenta; set => TotalVenta = value; }
    }
}
