namespace FrontCafeteriaMVC.Models
{
    public class HistorialCreditoFiltroViewModel
    {
        public List<HistorialCredito> Historial { get; set; } = new List<HistorialCredito>();

        public string NumeroControl { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }
}
