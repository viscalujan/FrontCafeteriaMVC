// IServicesAPI.cs
using FrontCafeteriaMVC.Models;

namespace FrontCafeteriaMVC.Services
{
    public interface IServicesAPI
    {
        Task<LoginResponse> LoginAsync(LoginRequest login);

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
        Task<(string base64, string downloadUrl)> RegenerarQRAsync(string numeroControl);


        Task<bool> TransferirCreditoAsync(TransferenciaCreditoDTO dto, string numeroControlEmisor);
        Task<bool> EnviarQRAsync(string correoDestino, string numeroControl);

        Task<List<HistorialCredito>> ObtenerHistorialFiltradoAsync(DateTime? desde, DateTime? hasta, string numeroControl);
        Task<byte[]> ExportarHistorialExcelAsync(DateTime? desde, DateTime? hasta, string numeroControl);

        //Task<List<ProductoDTO>> GetProductosAsync();
        Task<List<PedidoViewModel>> GetPedidosAsync();
        Task<bool> CambiarEstadoPedidoAsync(int idPedido, int nuevoEstado);
       // Task<object> CrearPedidoAsync(PedidoCreateDTO pedido);

        Task<string> CrearPedidoAsync(PedidoCreateDTO pedido);
        Task<bool> CancelarVentaAsync(int idVenta);
        Task<List<PedidoViewModel>> GetPedidosPorNumeroControlAsync(string numeroControl);
        Task<bool> SolicitarCodigoRecuperacionAsync(string correo);
        Task<bool> ValidarCodigoRecuperacionAsync(string correo, string codigo);
        Task<bool> GuardarNuevaContraAsync(string correo, string nuevaContra);
        Task<bool> RegistrarAdminAsync(AdminRegistroDTO admin);

    }
}