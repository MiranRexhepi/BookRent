using BookRental.Constants;
using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookRental.Features.Auth.Commands;

public class RefreshTokenCommand(UserManager<User> userManager, IConfiguration configuration, BookRentalContext context, TokenService tokenService)
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly BookRentalContext _context = context;
    private readonly TokenService _tokenService = tokenService;

    public async Task<object?> Execute(RefreshTokenDto dto)
    {
        // Find the refresh token
        var refreshTokenEntity = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

        if (refreshTokenEntity == null)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        // Validate refresh token
        if (refreshTokenEntity.IsRevoked)
            throw new UnauthorizedAccessException("Refresh token has been revoked.");

        if (refreshTokenEntity.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        var user = refreshTokenEntity.User;
        if (user == null)
            throw new UnauthorizedAccessException("User not found.");

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate new access token
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("userId", user.Id),
            new("tenantId", user.TenantId.ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("roleName", role));
        }

        var jwtKey = _configuration["Jwt:Key"];

        if (string.IsNullOrEmpty(jwtKey))
            throw new InvalidOperationException("JWT key is not configured.");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(4),
            signingCredentials: creds
        );

        // Generate new refresh token
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(7);

        // Revoke old refresh token
        refreshTokenEntity.IsRevoked = true;

        // Save new refresh token
        var newRefreshTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = user.Id,
            ExpiresAt = refreshTokenExpiry,
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        return new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            refreshToken = newRefreshToken
        };
    }
}

