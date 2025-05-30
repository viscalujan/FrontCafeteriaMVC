using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class UsuarioCreateDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }

        [JsonPropertyName("Contra")]
        public string NumeroControl { get; set; }

        public decimal Credito { get; set; }
    }
}
