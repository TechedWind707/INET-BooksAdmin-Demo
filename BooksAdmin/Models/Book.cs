//Singular Book model class to represent a book entity in the application. This class contains properties for the book's ID, title, author, genre, description, and year published. It serves as a data structure to hold information about individual books and can be used in various parts of the application, such as controllers and views.
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace BooksAdmin.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [Display(Prompt = "Title..")]
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string Genre { get; set; } = string.Empty;

        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        public int YearPublished { get; set; }

    }
}