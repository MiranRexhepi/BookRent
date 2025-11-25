using BookRental.Data;
using BookRental.Features.Dashboard.Queries;
using BookRental.Features.Rentals.Queries;
using BookRental.Features.Books.Queries;
using BookRental.Helpers;
using BookRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;

[Route("api/dashboard")]
[Authorize]
public class DashboardController(BookRentalContext context) : ControllerBase
{
    private readonly BookRentalContext _context = context;

    [HttpGet("stats")]
    public async Task<IActionResult> GetDashboardStats()
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var query = new GetDashboardStatsQuery(_context);

        var stats = await query.Execute(tenantId);

        return Ok(stats);
    }

    [HttpGet("daily-rentals")]
    public async Task<IActionResult> GetDailyRentalStats()
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var query = new GetDailyRentalStatsQuery(_context);

        var stats = await query.Execute(tenantId);

        return Ok(stats);
    }

    [HttpGet("rental-history")]
    public async Task<IActionResult> GetRentalHistory(
        [FromQuery] Pagination pagination)
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var query = new GetRentalHistoryQuery(_context);

        var history = await query.Execute(tenantId, pagination);

        return Ok(history);
    }

    [HttpGet("available-books")]
    public async Task<IActionResult> GetAvailableBooks(
        [FromQuery] Pagination pagination)
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var query = new GetTenantAvailableBooks(_context);

        var books = await query.Execute(tenantId, pagination, new BookFilters());

        return Ok(books);
    }
}

