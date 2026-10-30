using Microsoft.EntityFrameworkCore;

public class BooksAdminContext(DbContextOptions<BooksAdminContext> options) : DbContext(options)
{
    public DbSet<BooksAdmin.Models.Book> Book { get; set; } = default!;
    public DbSet<BooksAdmin.Models.Genre> Genre { get; set; } = default!;
    public DbSet<BooksAdmin.Models.AudienceCategory> AudienceCategory { get; set; } = default!;
}
