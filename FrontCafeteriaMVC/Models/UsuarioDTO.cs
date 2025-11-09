namespace FrontCafeteriaMVC.Models
{
    public class UsuarioDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string NumeroControl { get; set; } = string.Empty;
        public decimal Credito { get; set; }
    }
}
