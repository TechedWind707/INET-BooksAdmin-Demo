using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BooksAdmin.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        [ValidateNever]
        public List<Book> Books { get; set; } = new List<Book>();
    }
}
