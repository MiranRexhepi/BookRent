namespace BookRental.DTOs;

public class UpdateBookDto
{
    public required string Title { get; set; }
    public required string Genre { get; set; }
    public required string ISBN { get; set; }
    public required string Author { get; set; }
}
