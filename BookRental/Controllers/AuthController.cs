using BookRental.DTOs;
using BookRental.Features.Auth.Commands;
using BookRental.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;
public class AuthController(
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : ControllerBase
{
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost("/api/register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var command = new RegisterUserCommand(_userManager, _roleManager, _configuration);

        var token = await command.Execute(dto);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Invalid credentials.");

        return Ok(new { token });
    }

    [HttpPost("/api/login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var command = new LoginUserCommand(_userManager, _configuration);

        var token = await command.Execute(dto);

        if (string.IsNullOrEmpty(token))
            return Unauthorized("Invalid credentials.");

        return Ok(new { token });
    }
}