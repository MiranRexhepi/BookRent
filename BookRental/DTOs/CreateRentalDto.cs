namespace BookRental.DTOs;

public class CreateRentalDto
{
    public int BookId { get; set; }
    public int TenantId { get; set; }
    public DateTime RentalDate { get; set; }
    public DateTime ReturnedAt { get; set; }
}
