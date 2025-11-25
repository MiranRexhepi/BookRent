using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.DTOs;
using BookRental.Features.Auth.Commands;
using BookRental.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;
public class AuthController(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration,
    BookRentalContext context,
    TokenService tokenService) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _configuration = configuration;
    private readonly BookRentalContext _context = context;
    private readonly TokenService _tokenService = tokenService;

    [HttpPost("/api/register")]
    [Authorize]
    public async Task<IActionResult> Register(
        [FromBody] RegisterDto dto)
    {
        User.IsAdmin();

        var command = new RegisterUserCommand(_userManager, _roleManager, _configuration);

        var token = await command.Execute(dto, User.GetTenantId());

        return Ok(new { token });
    }

    [HttpPost("/api/login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto dto)
    {
        var command = new LoginUserCommand(_userManager, _configuration, _context, _tokenService);

        var token = await command.Execute(dto);

        return Ok(token);
    }

    [HttpPost("/api/refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto dto)
    {
        var command = new RefreshTokenCommand(_userManager, _configuration, _context, _tokenService);

        var token = await command.Execute(dto);

        return Ok(token);
    }
}