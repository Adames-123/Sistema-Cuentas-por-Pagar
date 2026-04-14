using Microsoft.AspNetCore.Mvc;
using Sistema_Documentos_por_Pagar.Data;
using Sistema_Documentos_por_Pagar.Models;

using Microsoft.EntityFrameworkCore;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var hoy = DateTime.Now;

        var docs = _context.DocumentosPagar
            .Include(d => d.Proveedor)
            .Where(d => d.Estado == true)
            .ToList();

        var model = new DashboardViewModel();

        
        model.TotalPorPagar = docs.Sum(x => x.Monto);
        model.CantidadFacturas = docs.Count;

        var vencidos = docs.Where(x => x.FechaDocumento < hoy).ToList();

        model.TotalVencido = vencidos.Sum(x => x.Monto);
        model.FacturasVencidas = vencidos.Count;

        model.ProximosPagos = docs
            .Where(x => x.FechaDocumento >= hoy)
            .OrderBy(x => x.FechaDocumento)
            .Take(5)
            .ToList();

   

        model.Meses = Enumerable.Range(1, 12).ToList();

        model.MontosPorMes = model.Meses
            .Select(m => docs
                .Where(x => x.FechaDocumento.HasValue && x.FechaDocumento.Value.Month == m)
                .Sum(x => x.Monto))
            .ToList();

        return View(model);
    }
}