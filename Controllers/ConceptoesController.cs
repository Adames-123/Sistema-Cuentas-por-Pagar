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
    public class ConceptoesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ConceptoesController(ApplicationDbContext context)
        {
            _context = context;
        }
   
        // GET: Conceptoes
        public async Task<IActionResult> Index()
        {
            return View(await _context.ConceptosPago.ToListAsync());
        }

        // GET: Conceptoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concepto = await _context.ConceptosPago
                .FirstOrDefaultAsync(m => m.IdConcepto == id);
            if (concepto == null)
            {
                return NotFound();
            }

            return View(concepto);
        }

        // GET: Conceptoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Conceptoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdConcepto,Descripcion,Estado")] Concepto concepto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(concepto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(concepto);
        }

        // GET: Conceptoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concepto = await _context.ConceptosPago.FindAsync(id);
            if (concepto == null)
            {
                return NotFound();
            }
            return View(concepto);
        }

        // POST: Conceptoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdConcepto,Descripcion,Estado")] Concepto concepto)
        {
            if (id != concepto.IdConcepto)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(concepto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ConceptoExists(concepto.IdConcepto))
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
            return View(concepto);
        }

        // GET: Conceptoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var concepto = await _context.ConceptosPago
                .FirstOrDefaultAsync(m => m.IdConcepto == id);
            if (concepto == null)
            {
                return NotFound();
            }

            return View(concepto);
        }

        // POST: Conceptoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var concepto = await _context.ConceptosPago.FindAsync(id);
            if (concepto != null)
            {
                _context.ConceptosPago.Remove(concepto);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleEstado(int id)
        {
            var concepto = await _context.ConceptosPago.FindAsync(id);

            if (concepto == null)
                return NotFound();

            concepto.Estado = !concepto.Estado;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool ConceptoExists(int id)
        {
            return _context.ConceptosPago.Any(e => e.IdConcepto == id);
        }
    }

}
