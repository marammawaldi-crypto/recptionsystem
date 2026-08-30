
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
using ReceptionSystem.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ReceptionSystem.Controllers
{
    [Authorize]
    public class VisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET: VISITORS
        // =====================================================

        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Visitors.ToListAsync()
            );
        }

        // =====================================================
        // GET: VISITORS/Details/5
        // =====================================================

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor =
                await _context.Visitors
                    .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // GET: VISITORS/Create
        // =====================================================

        public IActionResult Create()
        {
            return View();
        }

        // =====================================================
        // POST: VISITORS/Create
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Visitor visitor)
        {
            if (!ModelState.IsValid)
            {
                return View(visitor);
            }

            _context.Visitors.Add(visitor);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Visitor saved successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: VISITORS/Edit/5
        // =====================================================

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor =
                await _context.Visitors.FindAsync(id.Value);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // POST: VISITORS/Edit/5
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Visitor visitor)
        {
            if (id != visitor.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(visitor);
            }

            try
            {
                _context.Update(visitor);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Visitor updated successfully.";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Visitors
                    .Any(e => e.Id == visitor.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: VISITORS/Delete/5
        // =====================================================

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor =
                await _context.Visitors
                    .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // POST: VISITORS/Delete/5
        // =====================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitor =
                await _context.Visitors.FindAsync(id);

            if (visitor == null)
            {
                return NotFound();
            }

            _context.Visitors.Remove(visitor);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Visitor deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}

