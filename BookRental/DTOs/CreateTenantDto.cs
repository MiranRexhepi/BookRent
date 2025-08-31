namespace BookRental.DTOs;

public class CreateTenantDto
{
    public required string TenantName { get; set; }
    public required string UserEmail { get; set; }
    public required string Password { get; set; }
}
