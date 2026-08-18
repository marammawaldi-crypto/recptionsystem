
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Models;
using ReceptionSystem.Data;

namespace ReceptionSystem.Controllers
{
    public class QualificationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public QualificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: QUALIFICATIONS
        public async Task<IActionResult> Index()
        {
            return View(await _context.Qualifications.ToListAsync());
        }

        // GET: QUALIFICATIONS/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualification = await _context.Qualifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qualification == null)
            {
                return NotFound();
            }

            return View(qualification);
        }

        // GET: QUALIFICATIONS/Create
        // GET: Qualifications/Create
        public IActionResult Create(int jobApplicationId)
        {
            var qualification = new Qualification
            {
                JobApplicationId = jobApplicationId
            };

            return View(qualification);
        }

        // POST: QUALIFICATIONS/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Degree,Specialization,University,GraduationYear,JobApplicationId,JobApplication")] Qualification qualification)
        {
            if (ModelState.IsValid)
            {
                _context.Add(qualification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(qualification);
        }

        // GET: QUALIFICATIONS/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification == null)
            {
                return NotFound();
            }
            return View(qualification);
        }

        // POST: QUALIFICATIONS/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Degree,Specialization,University,GraduationYear,JobApplicationId")] Qualification qualification)
        {
            if (id != qualification.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(qualification);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QualificationExists(qualification.Id))
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
            return View(qualification);
        }

        // GET: QUALIFICATIONS/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var qualification = await _context.Qualifications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (qualification == null)
            {
                return NotFound();
            }

            return View(qualification);
        }

        // POST: QUALIFICATIONS/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            var qualification = await _context.Qualifications.FindAsync(id);
            if (qualification != null)
            {
                _context.Qualifications.Remove(qualification);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QualificationExists(int? id)
        {
            return _context.Qualifications.Any(e => e.Id == id);
        }
    }
}