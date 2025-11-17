namespace FrontCafeteriaMVC.Models
{
    public class LoginResponse
    {
        // LOGIN NORMAL
        public string token { get; set; }
        public string rol { get; set; }
        public string numeroControl { get; set; }
        public int iniciosSesion { get; set; }

        // PRIMER INICIO
        public bool requiereCambio { get; set; }
        public string modo { get; set; }
        public string mensaje { get; set; }
        public string tipoUsuario { get; set; }
    }

}

