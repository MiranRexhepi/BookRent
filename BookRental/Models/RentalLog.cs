namespace BookRental.Models;

public class RentalLog
{
    public int Id { get; set; }
    public int RentalId { get; set; }
    public Rental Rental { get; set; }
    public int RentalStatusId { get; set; }
    public RentalStatus RentalStatus { get; set; }
    public DateTime CreatedAt { get; set; }
}
