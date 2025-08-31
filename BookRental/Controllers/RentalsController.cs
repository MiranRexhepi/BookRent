using BookRental.Data;
using BookRental.Features.Rentals.Commands;
using BookRental.Features.Rentals.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;

[Route("api/rentals")]
[Authorize]
public class RentalsController(BookRentalContext context, WS.WebSocketManager wsManager) : ControllerBase
{
    private readonly BookRentalContext _context = context;
    private readonly WS.WebSocketManager _wsManager = wsManager;

    [HttpGet("history")]
    public async Task<IActionResult> GetRentalHistory()
    {
        var tenantId = User.FindFirst("tenantId")?.Value;
        var userId = User.FindFirst("userId")?.Value;

        if (string.IsNullOrEmpty(tenantId))
            return BadRequest("Tenant ID is missing.");

        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is missing.");

        var query = new GetRentalHistoryQuery(_context);

        var history = await query.Execute(int.Parse(tenantId));

        return Ok(history);
    }

    [HttpPost("books/{bookId}")]
    public async Task<IActionResult> RentBook(int bookId)
    {
        var tenantId = User.FindFirst("tenantId")?.Value;
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest("Tenant ID is missing.");
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is missing.");
        var command = new RentBookCommand(_context, _wsManager);
        try
        {
            await command.Execute(int.Parse(tenantId), bookId, userId);
            return Ok("Book rented successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }

    [HttpPut("{id}/books/{bookId}")]
    public async Task<IActionResult> ReturnBook(int id, int bookId)
    {
        var tenantId = User.FindFirst("tenantId")?.Value;
        var userId = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(tenantId))
            return BadRequest("Tenant ID is missing.");
        if (string.IsNullOrEmpty(userId))
            return BadRequest("User ID is missing.");
        var command = new ReturnBookCommand(_context);
        try
        {
            await command.Execute(int.Parse(tenantId), bookId, userId, id);
            return Ok("Book returned successfully.");
        }
        catch (Exception ex)
        {
            return BadRequest(new { Error = ex.Message });
        }
    }
}