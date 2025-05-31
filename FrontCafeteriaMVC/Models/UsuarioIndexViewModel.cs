namespace FrontCafeteriaMVC.Models
{
    public class UsuarioIndexViewModel
    {

        public List<UsuarioDTO> Usuarios { get; set; }
        public UsuarioRegistroDTO UsuarioRegistro { get; set; } = new();
        public AumentoCreditoDTO AumentoCredito { get; set; } = new();

    }
}