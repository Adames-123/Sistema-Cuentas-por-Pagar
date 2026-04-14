using Microsoft.EntityFrameworkCore;

namespace Sistema_Documentos_por_Pagar.Models
{
    public class DashboardViewModel
    {
        public decimal TotalPorPagar { get; set; }
        public int CantidadFacturas { get; set; }
        public decimal TotalVencido { get; set; }
        public int FacturasVencidas { get; set; }
        public string[] ConceptosLabels { get; set; } = Array.Empty<string>();
        public decimal[] ConceptosMontos { get; set; } = Array.Empty<decimal>();
        public List<DocumentoPorPagar> ProximosPagos { get; set; } = new();
        public List<int> Meses { get; set; } = new();
        public List<decimal> MontosPorMes { get; set; } = new();
    }
    public class TopProveedorVM
    {
        public string? Proveedor { get; set; }
        public decimal? Total { get; set; }
    }
}
