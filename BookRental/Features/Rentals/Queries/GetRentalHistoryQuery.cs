using BookRental.Data;
using BookRental.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Queries;

public class GetRentalHistoryQuery(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<IEnumerable<RentalLogDto>> Execute(int tenantId)
    {
        return await _context.RentalLogs
            .Where(log => log.Rental.Book.TenantId == tenantId)
            .Select(b =>
                new RentalLogDto
                {
                    Id = b.Id,
                    RentalId = b.RentalId,
                    RentalStatusId = b.RentalStatusId,
                    CreatedAt = b.CreatedAt
                })
            .ToListAsync();
    }
}
