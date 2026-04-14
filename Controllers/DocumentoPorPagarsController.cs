using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Documentos_por_Pagar.Models;
using Sistema_Documentos_por_Pagar.Data;

namespace Sistema_Documentos_por_Pagar.Models.Controllers
{
    public class DocumentoPorPagarsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DocumentoPorPagarsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DocumentoPorPagars
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.DocumentosPagar.Include(d => d.Concepto).Include(d => d.Proveedor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: DocumentoPorPagars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentoPorPagar = await _context.DocumentosPagar
                .Include(d => d.Concepto)
                .Include(d => d.Proveedor)
                .FirstOrDefaultAsync(m => m.IdDocumento == id);
            if (documentoPorPagar == null)
            {
                return NotFound();
            }

            return View(documentoPorPagar);
        }

        // GET: DocumentoPorPagars/Create
        public IActionResult Create()
        {
            ViewData["IdConcepto"] = new SelectList(_context.ConceptosPago, "IdConcepto", "Descripcion");
            ViewData["IdProveedor"] = new SelectList(_context.Proveedores, "IdProveedor", "Nombre");
            
            return View();
        }

        // POST: DocumentoPorPagars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdDocumento,IdProveedor,IdConcepto,Monto,NumeroDocumento,NumeroFactura,FechaDocumento,FechaRegistro,Estado")] DocumentoPorPagar documentoPorPagar)
        {
            if (ModelState.IsValid)
            {
                _context.Add(documentoPorPagar);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdConcepto"] = new SelectList(_context.ConceptosPago, "IdConcepto", "Descripcion", documentoPorPagar.IdConcepto);
            ViewData["IdProveedor"] = new SelectList(_context.Proveedores, "IdProveedor", "Nombre", documentoPorPagar.IdProveedor);
            
            return View(documentoPorPagar);
        }

        // GET: DocumentoPorPagars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentoPorPagar = await _context.DocumentosPagar.FindAsync(id);
            if (documentoPorPagar == null)
            {
                return NotFound();
            }
            ViewData["IdConcepto"] = new SelectList(_context.ConceptosPago, "IdConcepto", "Descripcion", documentoPorPagar.IdConcepto);
            ViewData["IdProveedor"] = new SelectList(_context.Proveedores, "IdProveedor", "Nombre", documentoPorPagar.IdProveedor);
            return View(documentoPorPagar);
        }

        // POST: DocumentoPorPagars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDocumento,IdProveedor,IdConcepto,Monto,NumeroDocumento,NumeroFactura,FechaDocumento,FechaRegistro,Estado")] DocumentoPorPagar documentoPorPagar)
        {
            if (id != documentoPorPagar.IdDocumento)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documentoPorPagar);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentoPorPagarExists(documentoPorPagar.IdDocumento))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdConcepto"] = new SelectList(_context.ConceptosPago, "IdConcepto", "Descripcion", documentoPorPagar.IdConcepto);
            ViewData["IdProveedor"] = new SelectList(_context.Proveedores, "IdProveedor", "Nombre", documentoPorPagar.IdProveedor);
            return View(documentoPorPagar);
        }

        // GET: DocumentoPorPagars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentoPorPagar = await _context.DocumentosPagar
                .Include(d => d.Concepto)
                .Include(d => d.Proveedor)
                .FirstOrDefaultAsync(m => m.IdDocumento == id);
            if (documentoPorPagar == null)
            {
                return NotFound();
            }

            return View(documentoPorPagar);
        }

        // POST: DocumentoPorPagars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documentoPorPagar = await _context.DocumentosPagar.FindAsync(id);
            if (documentoPorPagar != null)
            {
                _context.DocumentosPagar.Remove(documentoPorPagar);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var doc = await _context.DocumentosPagar.FindAsync(id);

            if (doc == null)
                return NotFound();

            doc.Estado = !doc.Estado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool DocumentoPorPagarExists(int id)
        {
            return _context.DocumentosPagar.Any(e => e.IdDocumento == id);
        }
    }
}
