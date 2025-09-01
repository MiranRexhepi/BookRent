using BookRental.Enums;

namespace BookRental.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }
    public int TenantId { get; set; }
    public string ISBN { get; set; }
    public BookStatus Status { get; set; }
    public int isDeleted { get; internal set; }
}
