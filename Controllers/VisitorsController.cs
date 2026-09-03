using ReceptionSystem.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
using ReceptionSystem.Models;
using ReceptionSystem.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ReceptionSystem.Controllers
{
    [Authorize]
    public class VisitorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionService _permissionService;

        public VisitorsController(
            ApplicationDbContext context,
            PermissionService permissionService)
        {
            _context = context;
            _permissionService = permissionService;
        }

        // =====================================================
        // GET: Visitors
        // =====================================================

        [Permission("Visitors.View")]
        public async Task<IActionResult> Index()
        {
            var visitors = await _context.Visitors
                .OrderByDescending(v => v.Id)
                .ToListAsync();

            // -------------------------------------------------
            // Check Delete Permission
            // -------------------------------------------------

            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var canDelete = false;

            if (!string.IsNullOrEmpty(userId))
            {
                canDelete =
                    await _permissionService.HasPermissionAsync(
                        userId,
                        "Visitors.Delete");
            }

            // إرسال النتيجة إلى View
            ViewBag.CanDelete = canDelete;

            return View(visitors);
        }

        // =====================================================
        // GET: Visitors/Details/5
        // =====================================================

        [Permission("Visitors.Details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // GET: Visitors/Create
        // =====================================================

        [Permission("Visitors.Create")]
        public IActionResult Create()
        {
            return View();
        }

        // =====================================================
        // POST: Visitors/Create
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Visitors.Create")]
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
        // GET: Visitors/Edit/5
        // =====================================================

        [Permission("Visitors.Edit")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors.FindAsync(id.Value);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // POST: Visitors/Edit/5
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Permission("Visitors.Edit")]
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
                if (!_context.Visitors.Any(e => e.Id == visitor.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // GET: Visitors/Delete/5
        // =====================================================

        [Permission("Visitors.Delete")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // =====================================================
        // POST: Visitors/Delete/5
        // =====================================================

        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Permission("Visitors.Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);

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