using Microsoft.EntityFrameworkCore;

public class BooksAdminContext(DbContextOptions<BooksAdminContext> options) : DbContext(options)
{
    public DbSet<BooksAdmin.Models.Book> Book { get; set; } = default!;
}
