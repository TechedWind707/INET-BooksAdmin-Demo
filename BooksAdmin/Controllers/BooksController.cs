using BooksAdmin.Models;
using BooksAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//using System.Net.Http;

public class BooksController : Controller
{
    private readonly BooksAdminContext _context;
    private readonly IAzureBlobService _blobService;

    //private readonly IHttpClientFactory httpClientFactory;

    public BooksController(BooksAdminContext context, IAzureBlobService azureBlobService)
    {
        _context = context;
        _blobService = azureBlobService;
        //_httpClientFactory = httpClientFactory;
    }

    //OpenLibrary availability helper method
    //private async Task<bool> ExistsInOpenLibraryAsync(string isbn)
    //{
    //    var client = _httpClientFactory.CreateClient();
    //    var url = $"https://openlibrary.org/api/books?bibkeys=ISBN:{isbn}&format=json";
    //    var response = await client.GetAsync(url);
    //    if (!response.IsSuccessStatusCode)
    //    {
    //        return false;
    //    }
    //    var content = await response.Content.ReadAsStringAsync();
    //    return !string.IsNullOrWhiteSpace(content) && content != "{}";
    //}

    // GET: BOOKS
    public async Task<IActionResult> Index()    
    {
        List<Book> books = await _context.Book
            .Include(m => m.Genre)
            .Include(m => m.AudienceCategory)
            .ToListAsync();

        return View(books);
    }

    //GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Book
            .Include (m => m.Genre)
            .Include(m => m.AudienceCategory)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    //GET: BOOKS/Create
    public IActionResult Create()
    {
        // create a select list for the genres and categories
        var genres = _context.Genre.ToList();
        ViewBag.GenreId = new SelectList(genres, "Id", "Title");
        var categories = _context.AudienceCategory.ToList();
        ViewBag.AudienceCategoryId = new SelectList(categories, "Id", "Title");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Author,GenreId,AudienceCategoryId,Description,YearPublished,Isbn")] Book book, IFormFile? ImageFile)
    {
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
        var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

        // Manually add a validation failure
        if (ImageFile == null || ImageFile.Length == 0
            )
        {
            ModelState.AddModelError("ImageFile", "The image file is required.");
        }

        if (!allowedTypes.Contains(ImageFile.ContentType) || extension is not ".jpg" and not ".joeg" and not ".png" and not ".webp")
        {
            ModelState.AddModelError("ImageFile", "Upload a JPG, PNG, or WebP image.");
        }
            if (ModelState.IsValid)
        {
            // Generate a unique file name for the uploaded image
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(ImageFile.FileName)}";
                // Upload the image to Azure Blob Storage and get the URL
                var imageUrl = await _blobService.UploadFileAsync(ImageFile, fileName);
                // Set the UploadedCoverUrl property of the book to the uploaded image URL
                book.UploadedCoverUrl = imageUrl;

            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        var genres = _context.Genre.ToList();
        ViewBag.GenreId = new SelectList(genres, "Id", "Title");
        var categories = _context.AudienceCategory.ToList();
        ViewBag.AudienceCategoryId = new SelectList(categories, "Id", "Title");
        return View(book);
    }


    //GET: BOOKS/Edit/5
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

        var genres = _context.Genre.ToList();
        ViewBag.GenreId = new SelectList(genres, "Id", "Title");
        var categories = _context.AudienceCategory.ToList();
        ViewBag.AudienceCategoryId = new SelectList(categories, "Id", "Title");

        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Author,GenreId,AudienceCategoryId,Description,YearPublished,Isbn")] Book book, IFormFile? ImageFile)
    {
        if (id != book.Id)
        {
            return NotFound();
        }

        if (ImageFile != null && ImageFile.Length > 0)
        {
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

            if (!allowedTypes.Contains(ImageFile.ContentType) || extension is not ".jpg" and not ".jpeg" and not ".png" and not ".webp" || ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Upload a JPG, PNG, or WebP image.");
            }
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
            existingBook.GenreId = book.GenreId;
            existingBook.AudienceCategoryId = book.AudienceCategoryId;
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
        
        var genres = _context.Genre.ToList();
        ViewBag.GenreId = new SelectList(genres, "Id", "Title");
        var categories = _context.AudienceCategory.ToList();
        ViewBag.AudienceCategoryId = new SelectList(categories, "Id", "Title");

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
