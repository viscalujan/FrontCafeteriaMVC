namespace FrontCafeteriaMVC.Models
{
    public class MiCuentaVM
    {

        public string NumeroControl { get; set; }
        public string Nombre { get; set; }
        public decimal Credito { get; set; }
        public List<HistorialCreditoVM> Historial { get; set; }
        public string QrBase64 { get; set; }      // para <img>
        public string QrDownloadUrl { get; set; } // para <a download>

    }
}
