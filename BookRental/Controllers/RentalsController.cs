using BookRental.Constants;
using BookRental.Data;
using BookRental.Features.Rentals.Commands;
using BookRental.Features.Rentals.Queries;
using BookRental.Helpers;
using BookRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace BookRental.Controllers;

[Route("api/rentals")]
[Authorize]
public class RentalsController(
    BookRentalContext context,
    Middleware.WebSocketManager wsManager) : ControllerBase
{
    private readonly BookRentalContext _context = context;
    private readonly Middleware.WebSocketManager _wsManager = wsManager;

    [HttpGet("history")]
    public async Task<IActionResult> GetRentalHistory(
        [FromQuery] Pagination pagination)
    {
        var tenantId = User.GetTenantId();

        var query = new GetRentalHistoryQuery(_context);

        var history = await query.Execute(tenantId, pagination);

        return Ok(history);
    }

    [HttpGet("current")]
    public async Task<IActionResult> GetCurrentRentalHistory()
    {
        var userId = User.GetUserId();

        var query = new GetAllRentals(_context);

        var history = await query.Execute(userId);

        return Ok(history);
    }

    [HttpPost("books/{bookId}")]
    public async Task<IActionResult> RentBook(
        [FromRoute] int bookId)
    {
        var tenantId = User.GetTenantId();

        var userId = User.GetUserId();

        var command = new RentBookCommand(_context, _wsManager);

        await command.Execute(tenantId, bookId, userId);

        return Ok(JsonSerializer.Serialize(Messages.BookRentedSuccessfully));
    }

    [HttpPut("{id}/books/{bookId}")]
    public async Task<IActionResult> ReturnBook(int id, int bookId)
    {
        var tenantId = User.GetTenantId();

        var userId = User.GetUserId();

        var command = new ReturnBookCommand(_context, _wsManager);

        await command.Execute(tenantId, bookId, userId, id);

        return Ok(JsonSerializer.Serialize(Messages.BookReturnedSuccessfully));
    }
}