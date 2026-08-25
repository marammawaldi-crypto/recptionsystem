using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Models;
using ReceptionSystem.Data;

public class VisitorsController : Controller
{
    private readonly ApplicationDbContext _context;

    public VisitorsController(ApplicationDbContext context)
    {
        _context = context;
    }


    // GET: Visitors
    public async Task<IActionResult> Index()
    {
        var visitors = await _context.Visitors
            .OrderByDescending(v => v.Id)
            .ToListAsync();

        return View(visitors);
    }


    // GET: Visitors/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(m => m.Id == id);

        if (visitor == null)
            return NotFound();

        return View(visitor);
    }


    // GET: Visitors/Create
    public IActionResult Create()
    {
        return View();
    }


    // POST: Visitors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Visitor visitor)
    {
        if (ModelState.IsValid)
        {
            _context.Add(visitor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(visitor);
    }


    // GET: Visitors/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var visitor = await _context.Visitors.FindAsync(id);

        if (visitor == null)
            return NotFound();

        return View(visitor);
    }


    // POST: Visitors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Visitor visitor)
    {
        if (id != visitor.Id)
            return NotFound();

        if (ModelState.IsValid)
        {
            _context.Update(visitor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(visitor);
    }
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var visitor = await _context.Visitors
            .FirstOrDefaultAsync(m => m.Id == id);

        if (visitor == null)
            return NotFound();

        return View(visitor);
    }


    // POST: Visitors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var visitor = await _context.Visitors.FindAsync(id);
        if (visitor != null)
        {
            _context.Visitors.Remove(visitor);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}