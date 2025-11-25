using System.ComponentModel.DataAnnotations;

namespace BookRental.DTOs;

public class RefreshTokenDto
{
    [Required(ErrorMessage = "Refresh token is required.")]
    public required string RefreshToken { get; set; }
}

