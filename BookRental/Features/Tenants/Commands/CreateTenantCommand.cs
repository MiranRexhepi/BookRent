using BookRental.Constants;
using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.DTOs;
using BookRental.Features.Auth.Commands;

namespace BookRental.Features.Tenants.Commands;

public class CreateTenantCommand(
    BookRentalContext context,
    RegisterUserCommand registerUserCommand)
{
    private readonly BookRentalContext _context = context;
    private readonly RegisterUserCommand _registerUserCommand = registerUserCommand;

    public async Task<string> Execute(CreateTenantDto dto)
    {
        var tenant = new Tenant
        {
            Name = dto.TenantName
        };

        await _context.Tenants.AddAsync(tenant);
        await _context.SaveChangesAsync();

        var registerDto = new RegisterDto
        {
            Email = dto.UserEmail,
            Password = dto.Password,
            Role = UserRoles.Admin
        };

        var token = await _registerUserCommand.Execute(registerDto, tenant.Id);

        return token;
    }
}
