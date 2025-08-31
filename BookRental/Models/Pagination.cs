using System.ComponentModel.DataAnnotations;

namespace BookRental.Models;

public class Pagination
{
    [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1.")]
    public int PageNumber { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
    public int PageSize { get; set; } = 10;

    [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters.")]
    public string? Search { get; set; }
}