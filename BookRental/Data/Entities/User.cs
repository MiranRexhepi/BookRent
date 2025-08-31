using Microsoft.AspNetCore.Identity;

namespace BookRental.Data.Entities;

public class User : IdentityUser
{
    public int TenantId { get; set; }
    public Tenant Tenant { get; set; }
    public ICollection<Rental> Rentals { get; set; } = [];
    public bool IsDeleted { get; set; }
}