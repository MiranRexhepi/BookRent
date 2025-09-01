using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using BookRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Queries;

public class GetRentalHistoryQuery(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<PaginatedResponse<RentalDto>> Execute(int tenantId, Pagination pagination)
    {
        var query = _context.RentalLogs
            .Where(log => log.Rental.Book.TenantId == tenantId)
            .Select(b =>
               new RentalDto
               {
                   Id = b.Id,
                   BookId = b.Rental.BookId,
                   UserId = b.Rental.UserId,
                   RentedAt = b.Rental.RentedAt,
                   ReturnedAt = (RentalStatus)b.RentalStatusId == RentalStatus.Returned ? b.Rental.ReturnedAt : null,
                   RentalStatus = (RentalStatus)b.RentalStatusId
               })
            .OrderByDescending(x => x.Id);

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginatedResponse<RentalDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}
