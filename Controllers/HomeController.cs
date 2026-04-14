using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Documentos_por_Pagar.Data;
using Sistema_Documentos_por_Pagar.Models;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var hoy = DateTime.Today;

        var documentos = await _context.DocumentosPagar
            .Include(d => d.Proveedor)
            .Include(d => d.Concepto)
            .ToListAsync();

        var vm = new DashboardViewModel
        {
            // Tarjetas
            TotalPorPagar = documentos.Sum(d => d.Monto),
            CantidadFacturas = documentos.Count,
            TotalVencido = documentos
                                .Where(d => d.FechaDocumento < hoy)
                                .Sum(d => d.Monto),
            FacturasVencidas = documentos
                                .Count(d => d.FechaDocumento < hoy),

            // Próximos pagos (los 5 más próximos)
            ProximosPagos = documentos
                                .Where(d => d.FechaDocumento >= hoy)
                                .OrderBy(d => d.FechaDocumento)
                                .Take(5)
                                .ToList(),

            // Gráfica dona — por concepto
            ConceptosLabels = documentos
                                .GroupBy(d => d.Concepto?.Descripcion ?? "Sin concepto")
                                .Select(g => g.Key)
                                .ToArray(),
            ConceptosMontos = documentos
                                .GroupBy(d => d.Concepto?.Descripcion ?? "Sin concepto")
                                .Select(g => g.Sum(d => d.Monto))
                                .ToArray(),

            // Gráfica barras — últimos 6 meses
            Meses = Enumerable.Range(0, 6)
                        .Select(i => DateTime.Today.AddMonths(-5 + i).Month)
                        .ToList(),
            MontosPorMes = Enumerable.Range(0, 6)
                        .Select(i => {
                            var mes = DateTime.Today.AddMonths(-5 + i);
                            return documentos
                                .Where(d => d.FechaDocumento?.Month == mes.Month
                                         && d.FechaDocumento?.Year == mes.Year)
                                .Sum(d => d.Monto);
                        })
                        .ToList()
        };

        return View(vm);
    }
}