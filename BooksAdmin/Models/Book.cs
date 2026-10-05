using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksAdmin.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [Display(Prompt = "Title..")]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        public string Genre { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Year must be between 1900 and 2100")]
        public int YearPublished { get; set; }

        [Display(Prompt = "Optional: add it to fetch the cover image.")]
        [RegularExpression(
        @"^(?:ISBN(?:-1[03])?:?\s)?(?:(?:97[89][- ]?)?[0-9]{1,5}[- ]?[0-9]+[- ]?[0-9]+[- ]?[0-9X]|[0-9]{9}[0-9X])$",
        ErrorMessage = "Invalid ISBN format.")]
        public string? Isbn { get; set; } = string.Empty;
        
        [NotMapped]
        public string? CoverImageUrl =>
       string.IsNullOrWhiteSpace(Isbn) ? null
       : $"https://covers.openlibrary.org/b/isbn/{Isbn.Replace("-", "").Replace(" ", "")}-M.jpg?default=false";
    }
}
