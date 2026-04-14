using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Documentos_por_Pagar.Data;
using Sistema_Documentos_por_Pagar.Models;
using Sistema_Documentos_por_Pagar.Services;

namespace Sistema_Documentos_por_Pagar.Controllers
{
    public class ContabilizacionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ContabilidadService _contabilidadService;

        public ContabilizacionController(
            ApplicationDbContext context,
            ContabilidadService contabilidadService)
        {
            _context = context;
            _contabilidadService = contabilidadService;
        }

        // GET: Contabilizacion/Index
        public async Task<IActionResult> Index(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.DocumentosPagar
                .Include(d => d.Proveedor)
                .Include(d => d.Concepto)
                .Where(d => d.IdAsiento == null && d.Estado == true)
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(d => d.FechaDocumento >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(d => d.FechaDocumento <= fechaHasta.Value);

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(await query.ToListAsync());
        }

        // POST: Contabilizacion/Contabilizar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contabilizar(List<int> idsSeleccionados)
        {
            if (idsSeleccionados == null || !idsSeleccionados.Any())
            {
                TempData["Error"] = "Debe seleccionar al menos un documento para contabilizar.";
                return RedirectToAction(nameof(Index));
            }

            var documentos = await _context.DocumentosPagar
                .Include(d => d.Concepto)
                .Include(d => d.Proveedor)
                .Where(d => idsSeleccionados.Contains(d.IdDocumento) && d.IdAsiento == null)
                .ToListAsync();

            if (!documentos.Any())
            {
                TempData["Error"] = "No se encontraron documentos validos para contabilizar.";
                return RedirectToAction(nameof(Index));
            }

            int exitosos = 0;
            int fallidos = 0;

            foreach (var doc in documentos)
            {
                // Pasar el documento completo al servicio — él construye el request
                var respuesta = await _contabilidadService.EnviarAsientoAsync(doc);

                if (respuesta.Exitoso)
                {
                    doc.IdAsiento = respuesta.IdAsiento ?? -1; // -1 si exitoso pero sin ID
                    _context.Update(doc);
                    exitosos++;
                }
                else
                {
                    fallidos++;
                }
            }

            await _context.SaveChangesAsync();

            if (fallidos == 0)
                TempData["Success"] = $"{exitosos} documento(s) contabilizado(s) correctamente.";
            else
                TempData["Warning"] = $"{exitosos} contabilizado(s), {fallidos} fallaron. Verifique la conexion con Contabilidad.";

            return RedirectToAction(nameof(Index));
        }

        // GET: Contabilizacion/Contabilizados
        public async Task<IActionResult> Contabilizados(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            var query = _context.DocumentosPagar
                .Include(d => d.Proveedor)
                .Include(d => d.Concepto)
                .Where(d => d.IdAsiento != null)
                .AsQueryable();

            if (fechaDesde.HasValue)
                query = query.Where(d => d.FechaDocumento >= fechaDesde.Value);

            if (fechaHasta.HasValue)
                query = query.Where(d => d.FechaDocumento <= fechaHasta.Value);

            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            return View(await query.ToListAsync());
        }
    }
}
