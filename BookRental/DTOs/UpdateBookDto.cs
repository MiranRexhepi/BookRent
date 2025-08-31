using System.ComponentModel.DataAnnotations;

namespace BookRental.DTOs;

public class UpdateBookDto
{
    [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
    public required string Title { get; set; }

    [StringLength(50, ErrorMessage = "Genre cannot exceed 50 characters.")]
    public required string Genre { get; set; }

    [RegularExpression(@"^(?:\d{9}[\dXx]|\d{13})$", ErrorMessage = "Invalid ISBN format.")]
    public required string ISBN { get; set; }

    [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters.")]
    public required string Author { get; set; }
}