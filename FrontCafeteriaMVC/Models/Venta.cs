namespace FrontCafeteriaMVC.Models
{
    public class Venta
    {
        public int Id { get; set; }
        public int? UsuarioId { get; set; }
        public string MetodoPago { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<DetalleVenta> Detalles { get; set; } = new();
    }
}