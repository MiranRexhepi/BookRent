using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.DTOs;
using BookRental.Features.Auth.Commands;
using BookRental.Features.Tenants.Commands;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;

[Route("api/tenants")]
public class TenantsController(
    BookRentalContext context,
    UserManager<User> userManager,
    RoleManager<IdentityRole> roleManager,
    IConfiguration configuration) : ControllerBase
{
    private readonly BookRentalContext _context = context;
    private readonly UserManager<User> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly IConfiguration _configuration = configuration;

    [HttpPost]
    public async Task<IActionResult> CreateTenant([FromBody] CreateTenantDto dto)
    {
        try
        {
            var registerUserCommand = new RegisterUserCommand(_userManager, _roleManager, _configuration);

            var command = new CreateTenantCommand(_context, registerUserCommand);

            var token = await command.Execute(dto);

            return Ok(new { Token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}