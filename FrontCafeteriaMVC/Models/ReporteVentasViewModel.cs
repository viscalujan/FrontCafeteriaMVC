
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System;
using System.Text.Json.Serialization;

namespace FrontCafeteriaMVC.Models
{
    public class ReporteVentasViewModel
    {
        public DateTime? Desde { get; set; }
        public DateTime? Hasta { get; set; }
        public List<DetalleVentaView> Detalles { get; set; } = new();

        public List<VentaAgrupada> VentasAgrupadas { get; set; } = new();
        public decimal TotalGeneral => VentasAgrupadas.Sum(v => v.TotalVenta);
    }

    public class DetalleVentaView
    {
        [JsonPropertyName("ventaId")]
        public int VentaId { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("productoNombre")]
        public string ProductoNombre { get; set; }

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }

        [JsonPropertyName("precioUnitario")]
        public decimal PrecioUnitario { get; set; }

        [JsonIgnore] // No viene del API, es calculado
        public decimal Total => Cantidad * PrecioUnitario;

        [JsonPropertyName("metodoPago")]
        public string MetodoPago { get; set; }

        public List<DetalleVentaView> Detalles { get; set; }

        public decimal TotalVenta => Detalles.Sum(d => d.Total);

    }

    public class VentaAgrupada
    {
        public int VentaId { get; set; }
        public DateTime Fecha { get; set; }
        public string MetodoPago { get; set; }
        public List<DetalleVentaView> Detalles { get; set; }
        public decimal TotalVenta => Detalles.Sum(d => d.Total);
    }


}
