namespace FrontCafeteriaMVC.Models
{
    
        public class VentasViewModel
        {
        public List<Producto> Productos { get; set; }
        public List<DetalleVenta> Carrito { get; set; }

        public Dictionary<int, Producto> ProductosDic { get; set; } = new();
        public Ticket? Ticket { get; set; }
        public string? Buscar { get; set; }

        public decimal Total { get; set; }

    }


}
