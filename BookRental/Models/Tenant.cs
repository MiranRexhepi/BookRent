namespace BookRental.Models;

public class Tenant
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public ICollection<Book> Books { get; set; } = [];
    public bool IsDeleted { get; set; }
}
