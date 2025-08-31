namespace BookRental.DTOs;

public class RentalLogDto
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public int RentalStatusId { get; set; }
    public DateTime CreatedAt { get; set; }
}
