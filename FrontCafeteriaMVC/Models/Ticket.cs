namespace FrontCafeteriaMVC.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaVenta { get; set; }
        public string Metodo { get; set; } = string.Empty;
    }
}
