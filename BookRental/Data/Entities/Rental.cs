namespace BookRental.Data.Entities;

public class Rental
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; }
    public required string UserId { get; set; }
    public User User { get; set; }
    public DateTime RentedAt { get; set; }
    public DateTime? ReturnedAt { get; set; }
    public int RentalStatusId { get; set; }
    public RentalStatus RentalStatus { get; set; }
    public ICollection<RentalLog> RentalLogs { get; set; } = [];
}
