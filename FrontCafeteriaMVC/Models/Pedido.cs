using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontCafeteriaMVC.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        [Column("id_pedidos")]
        public int IdPedidos { get; set; }

        [Column("FK_id_usuario")]
        public int FkIdUsuario { get; set; }

        [Column("fecha_pedido")]
        public DateTime FechaPedido { get; set; }

        [Column("total_pedido")]
        public decimal? TotalPedido { get; set; }

        [Column("FK_id_estado")]
        public int FkIdEstado { get; set; }

        public virtual ICollection<PedidoDetalle> PedidoDetalles { get; set; } = new List<PedidoDetalle>();
    }
}
