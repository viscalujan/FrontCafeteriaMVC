namespace FrontCafeteriaMVC.Models
{
    public class PedidoViewModel
    {
        public int IdPedidos { get; set; }
        public string Alumno { get; set; }
        public string NumeroControl { get; set; }
        public string Estado { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal TotalPedido { get; set; }
        public List<PedidoDetalleViewModel> Detalles { get; set; } = new();
    }
}
