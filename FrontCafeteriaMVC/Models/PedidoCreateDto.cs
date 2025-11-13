namespace FrontCafeteriaMVC.Models
{
    public class PedidoCreateDTO
    {
        public int UsuarioId { get; set; }
        public List<PedidoDetalleDTO> Detalles { get; set; } = new();
    }
}
