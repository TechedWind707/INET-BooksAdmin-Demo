using BooksAdmin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class AudienceCategoriesController : Controller
{
    private readonly BooksAdminContext _context;

    public AudienceCategoriesController(BooksAdminContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        return View(await _context.AudienceCategory.OrderBy(a => a.Title).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceCategory = await _context.AudienceCategory
            .FirstOrDefaultAsync(a => a.Id == id);
        if (audienceCategory == null)
        {
            return NotFound();
        }

        return View(audienceCategory);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title")] AudienceCategory audienceCategory)
    {
        if (ModelState.IsValid)
        {
            _context.Add(audienceCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(audienceCategory);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceCategory = await _context.AudienceCategory.FindAsync(id);
        if (audienceCategory == null)
        {
            return NotFound();
        }

        return View(audienceCategory);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title")] AudienceCategory audienceCategory)
    {
        if (id != audienceCategory.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(audienceCategory);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AudienceCategoryExists(audienceCategory.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        return View(audienceCategory);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var audienceCategory = await _context.AudienceCategory
            .FirstOrDefaultAsync(a => a.Id == id);
        if (audienceCategory == null)
        {
            return NotFound();
        }

        return View(audienceCategory);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var audienceCategory = await _context.AudienceCategory.FindAsync(id);
        if (audienceCategory != null)
        {
            _context.AudienceCategory.Remove(audienceCategory);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool AudienceCategoryExists(int id)
    {
        return _context.AudienceCategory.Any(a => a.Id == id);
    }
}
