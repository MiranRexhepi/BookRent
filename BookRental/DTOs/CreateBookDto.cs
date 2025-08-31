namespace BookRental.DTOs;

public class CreateBookDto
{
    public required string Title { get; set; }
    public required string Genre { get; set; }
    public required string ISBN { get; set; }
    public int TenantId { get; set; }
    public required string Author { get; set; }
}
