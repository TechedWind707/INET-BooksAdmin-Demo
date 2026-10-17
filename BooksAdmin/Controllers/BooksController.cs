using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BooksAdmin.Models;
using BooksAdmin.Services;

public class BooksController : Controller
{
    private readonly BooksAdminContext _context;
    private readonly IAzureBlobService _blobService;

    public BooksController(BooksAdminContext context, IAzureBlobService azureBlobService)
    {
        _context = context;
        _blobService = azureBlobService;
    }

    public async Task<IActionResult> Index()    
    {
        return View(await _context.Book.OrderByDescending(b => b.YearPublished).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Author,Genre,Description,YearPublished,Isbn")] Book book, IFormFile? ImageFile)
    {
        if (ModelState.IsValid)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                // Generate a unique file name for the uploaded image
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                // Upload the image to Azure Blob Storage and get the URL
                var imageUrl = await _blobService.UploadFileAsync(ImageFile, fileName);
                // Set the UploadedCoverUrl property of the book to the uploaded image URL
                book.UploadedCoverUrl = imageUrl;
            }
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Author,Genre,Description,YearPublished,Isbn")] Book book, IFormFile? ImageFile)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var existingBook = await _context.Book.FindAsync(id);
            if(existingBook == null)
            {
                return NotFound();
            }

            existingBook.Title = book.Title;
            existingBook.Author = book.Author;
            existingBook.Genre = book.Genre;
            existingBook.Description = book.Description;
            existingBook.YearPublished = book.YearPublished;
            existingBook.Isbn = book.Isbn;

            // 3) If a new file was uploaded, create a GUID filename, upload it and replace the URL.
            //    If no file was chosen, leave existingBook.UploadedCoverUrl untouched.
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                var imageUrl = await _blobService.UploadFileAsync(ImageFile, fileName);
                existingBook.UploadedCoverUrl = imageUrl;
            }

            try
            {
                //_context.Update(book);
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
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
        return View(book);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Book.FindAsync(id);
        if (book != null)
        {
            _context.Book.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Book.Any(e => e.Id == id);
    }
}
