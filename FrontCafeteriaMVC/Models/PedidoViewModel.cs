using Newtonsoft.Json;

namespace FrontCafeteriaMVC.Models
{
    public class PedidoViewModel
    {
        [JsonProperty("Id")]
        public int IdPedidos { get; set; }

        [JsonProperty("Alumno")]
        public string Alumno { get; set; }

        [JsonProperty("NumeroControl")]
        public string NumeroControl { get; set; }

        [JsonProperty("Estado")]
        public string Estado { get; set; }

        [JsonProperty("Fecha")]
        public DateTime FechaPedido { get; set; }

        [JsonProperty("Total")]
        public decimal TotalPedido { get; set; }

        [JsonProperty("Detalles")]
        public List<PedidoDetalleViewModel> Detalles { get; set; } = new();
    }
}
