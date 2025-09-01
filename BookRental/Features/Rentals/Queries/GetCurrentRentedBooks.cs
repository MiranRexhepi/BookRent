using BookRental.Data;
using BookRental.DTOs;
using BookRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Queries;

public class GetCurrentRentedBooks(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<PaginatedResponse<RentalLogDto>> Execute(int tenantId, Pagination pagination)
    {
        var query = _context.RentalLogs
            .Where(log => log.Rental.Book.TenantId == tenantId)
            .Select(b =>
                new RentalLogDto
                {
                    Id = b.Id,
                    RentalId = b.RentalId,
                    RentalStatusId = b.RentalStatusId,
                    CreatedAt = b.CreatedAt
                });

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<RentalLogDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}
