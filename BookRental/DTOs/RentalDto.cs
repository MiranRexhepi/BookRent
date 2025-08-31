using BookRental.Enums;

namespace BookRental.DTOs;

public class RentalDto
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public required string UserId { get; set; }
    public DateTime RentedAt { get; set; }
    public RentalStatus RentalStatus { get; set; }
}
