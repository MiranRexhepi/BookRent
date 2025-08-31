namespace BookRental.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public required string Genre { get; set; }
    public required string ISBN { get; set; }
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public int BookStatusId { get; set; }
    public BookStatus BookStatus { get; set; }
    public bool IsDeleted { get; set; }
}
