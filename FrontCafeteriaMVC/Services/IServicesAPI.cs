// IServicesAPI.cs
using FrontCafeteriaMVC.Models;

namespace FrontCafeteriaMVC.Services
{
    public interface IServicesAPI
    {
        Task<(string token, string rol, string? numeroControl)> LoginAsync(LoginRequest login);

        Task<List<Producto>> GetProductosAsync();
        Task<Producto> GetProductoByIdAsync(int id);

        Task<bool> CrearVentaAsync(VentaCreate venta);
        Task<Ticket> CrearVentaConTicketAsync(VentaCreate venta);

        Task<bool> CrearProductoAsync(Producto producto);
        Task<bool> ActualizarProductoAsync(Producto producto);

        Task<List<UsuarioDTO>> GetUsuariosAsync();
        Task<bool> RegistrarUsuarioAsync(UsuarioRegistroDTO usuario);

        Task<Usuario> GetUsuarioPorNumeroControlAsync(string numeroControl);
        Task<bool> ActualizarCreditoUsuarioAsync(int id, decimal nuevoCredito);

        Task<Ticket?> GenerarVentaAsync(VentaCreate venta, string token);

        Task<bool> AumentarCreditoAsync(AumentoCreditoDTO dto);

        Task<List<HistorialCredito>> ObtenerHistorialCreditoGeneralAsync();
        Task<List<HistorialCredito>> ObtenerHistorialCreditoAsync(string numeroControl); Task<List<Venta>> GetVentasFiltradasAsync(DateTime? desde, DateTime? hasta);


        Task<List<DetalleVentaView>> GetReporteDetalladoAsync(DateTime? desde, DateTime? hasta);
        Task<List<VentaAgrupada>> GetVentasAgrupadasAsync(DateTime? desde, DateTime? hasta);

        Task<byte[]> GenerarExcelReporteAsync(DateTime? desde, DateTime? hasta);

        Task<string> PagarLiquidacionAsync(PagoLiquidacionDTO dto);

        Task<decimal?> ObtenerCreditoAsync(string numeroControl);
        Task<List<HistorialCreditoVM>> ObtenerHistorialAsync(string numeroControl);
        Task<(string base64, string downloadUrl)> ObtenerQrAsync(string numeroControl);

        Task<bool> VerificarNumeroControlExistenteAsync(string numeroControl);
        Task<bool> VerificarCorreoExistenteAsync(string correo);
        Task<List<string>> ObtenerNumerosControlAsync();
        Task<List<string>> ObtenerCorreosAsync();

        Task<decimal> ObtenerCreditoLiquidacionAsync();


    }
}