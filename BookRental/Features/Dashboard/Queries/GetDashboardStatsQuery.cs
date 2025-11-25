using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Dashboard.Queries;

public class GetDashboardStatsQuery(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<DashboardStatsDto> Execute(int tenantId)
    {
        var now = DateTime.UtcNow;
        var startOfMonth = new DateTime(now.Year, now.Month, 1);
        var thirtyDaysAgo = now.AddDays(-30);

        var activeUsersThisMonth = await _context.Rentals
            .Where(r => r.Book.TenantId == tenantId
                && r.RentedAt >= startOfMonth
                && r.RentedAt < now)
            .Select(r => r.UserId)
            .Distinct()
            .CountAsync();

        var rentedBooksAtMoment = await _context.Rentals
            .Where(r => r.Book.TenantId == tenantId
                && !r.Book.IsDeleted
                && (r.RentalStatusId == (int)RentalStatus.Rented || r.RentalStatusId == (int)RentalStatus.Overdue))
            .CountAsync();

        var overdueBooks = await _context.Rentals
            .Where(r => r.Book.TenantId == tenantId
                && !r.Book.IsDeleted
                && r.RentalStatusId == (int)RentalStatus.Overdue)
            .CountAsync();

        var activeUserIds = await _context.Rentals
            .Where(r => r.Book.TenantId == tenantId
                && r.RentedAt >= thirtyDaysAgo)
            .Select(r => r.UserId)
            .Distinct()
            .ToListAsync();

        var allUserIds = await _context.Users
            .Where(u => u.TenantId == tenantId && !u.IsDeleted)
            .Select(u => u.Id)
            .ToListAsync();

        var activeUserSet = new HashSet<string>(activeUserIds);
        var passiveUsers = allUserIds.Count(u => !activeUserSet.Contains(u));

        var totalBooksAddedThisMonth = await _context.Books
            .Where(b => b.TenantId == tenantId && !b.IsDeleted)
            .CountAsync();

        var totalBooksAvailable = await _context.Books
            .Where(b => b.TenantId == tenantId
                && !b.IsDeleted
                && b.BookStatusId == (int)BookStatus.Available)
            .CountAsync();

        return new DashboardStatsDto
        {
            ActiveUsersThisMonth = activeUsersThisMonth,
            RentedBooksAtMoment = rentedBooksAtMoment,
            OverdueBooks = overdueBooks,
            PassiveUsers = passiveUsers,
            TotalBooksAddedThisMonth = totalBooksAddedThisMonth,
            TotalBooksAvailable = totalBooksAvailable
        };
    }
}

