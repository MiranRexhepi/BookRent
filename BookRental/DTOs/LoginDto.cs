using System.ComponentModel.DataAnnotations;

namespace BookRental.DTOs;
public class LoginDto
{
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public required string Email { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must contain at least one uppercase letter, one number, and one special character.")]
    public required string Password { get; set; }
}
