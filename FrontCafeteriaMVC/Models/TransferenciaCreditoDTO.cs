namespace FrontCafeteriaMVC.Models
{
    public class TransferenciaCreditoDTO
    {
        public string NumeroControlReceptor { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public string ContrasenaEmisor { get; set; } = null!;
    }
}
