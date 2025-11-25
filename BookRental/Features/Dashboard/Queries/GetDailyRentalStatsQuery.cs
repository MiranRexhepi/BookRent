using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Dashboard.Queries;

public class GetDailyRentalStatsQuery(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<List<DailyRentalStatsDto>> Execute(int tenantId)
    {
        var now = DateTime.UtcNow;
        var thirtyDaysAgo = now.AddDays(-30);

        var rentalLogs = await _context.RentalLogs
            .Where(log => log.Rental.Book.TenantId == tenantId
                && log.CreatedAt >= thirtyDaysAgo
                && log.CreatedAt < now)
            .Select(log => new
            {
                Date = log.CreatedAt.Date,
                StatusId = log.RentalStatusId
            })
            .ToListAsync();

        var dailyStats = rentalLogs
            .GroupBy(log => log.Date)
            .Select(g => new DailyRentalStatsDto
            {
                Date = g.Key,
                RentsCount = g.Count(x => x.StatusId == (int)RentalStatus.Rented),
                ReturnsCount = g.Count(x => x.StatusId == (int)RentalStatus.Returned)
            })
            .OrderBy(x => x.Date)
            .ToList();

        var allDates = Enumerable.Range(0, 30)
            .Select(i => thirtyDaysAgo.AddDays(i).Date)
            .ToList();

        var result = allDates
            .Select(date => dailyStats.FirstOrDefault(d => d.Date == date)
                ?? new DailyRentalStatsDto
                {
                    Date = date,
                    RentsCount = 0,
                    ReturnsCount = 0
                })
            .ToList();

        return result;
    }
}

