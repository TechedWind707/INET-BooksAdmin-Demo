using BooksAdmin;
using BooksAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[Authorize]
public class BookReviewsController : Controller
{
    private readonly BooksAdminContext _context;

    public BookReviewsController(BooksAdminContext context)
    {
        _context = context;
    }

    // GET: BookReviews
    [Authorize]
    public async Task<IActionResult> Index()
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var reviews = await _context.BookReview.Where(review => review.CreatedBy == User.GetUserId())
            .Include(review => review.Book)
            .ToListAsync();

        return View(reviews);
    }

    // GET: BookReviews/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookReview = await _context.BookReview
            .Include(review => review.Book)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (bookReview == null)
        {
            return NotFound();
        }

        return View(bookReview);
    }

    // GET: BookReviews/Create
    public async Task<IActionResult> Create()
    {
        await PopulateBooksAsync();
        return View();
    }
    
    // POST: BookReviews/Create
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("BookId,Description,Rating,IsPublished")] BookReview bookReview)
    {
        var userId = User.GetUserId();
        var alreadyReviewed = await _context.BookReview.AnyAsync (review => review.BookId == bookReview.BookId &&
          review.CreatedBy == userId);

        if (alreadyReviewed)
        {
            ModelState.AddModelError(
                "BookId",
                "You have already submitted a review for this book."
                );
        }

        if (!ModelState.IsValid)
        {
            await PopulateBooksAsync(bookReview.BookId);
            return View(bookReview);
        }

        bookReview.CreatedBy = userId
            ?? throw new InvalidOperationException("User is not authenticated");
        bookReview.CreatedDate = DateTime.UtcNow;

        _context.Add(bookReview);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: BookReviews/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookReview = await _context.BookReview.FindAsync(id);
        if (bookReview == null)
        {
            return NotFound();
        }

        if (bookReview.IsPublished || bookReview.CreatedBy != User.GetUserId())
        {
            return NotFound();
        }

        await PopulateBooksAsync(bookReview.BookId);
        return View(bookReview);
    }

    // POST: BookReviews/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,Description,Rating,IsPublished")] BookReview bookReview)
    {
        if (id != bookReview.Id)
        {
            return NotFound();
        }

        var existingReview = await _context.BookReview.FindAsync(id);
        if (existingReview == null || existingReview.IsPublished || existingReview.CreatedBy != User.GetUserId())
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                existingReview.BookId = bookReview.BookId;
                existingReview.Description = bookReview.Description;
                existingReview.Rating = bookReview.Rating;
                existingReview.IsPublished = bookReview.IsPublished;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookReviewExists(bookReview.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        await PopulateBooksAsync(bookReview.BookId);
        return View(bookReview);
    }

    // GET: BookReviews/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var bookReview = await _context.BookReview
            .Include(review => review.Book)
            .FirstOrDefaultAsync(review => review.Id == id);

        if (bookReview == null)
        {
            return NotFound();
        }

        return View(bookReview);
    }

    // POST: BookReviews/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var bookReview = await _context.BookReview.FindAsync(id);

        if (bookReview.IsPublished)
        {
            TempData["ErrorMessage"] =
    "Published reviews cannot be deleted.";
            return RedirectToAction(nameof(Index));
        }

        if (bookReview != null)
        {
            _context.BookReview.Remove(bookReview);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool BookReviewExists(int id)
    {
        return _context.BookReview.Any(review => review.Id == id);
    }

    private async Task PopulateBooksAsync(int? selectedBookId = null)
    {
        var books = await _context.Book
            .OrderBy(book => book.Title)
            .ToListAsync();

        ViewData["BookId"] = new SelectList(books, "Id", "Title", selectedBookId);
    }
}
