using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Queries;

public class GetAllRentals(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<IEnumerable<RentalDto>> Execute()
    {
        return await _context.Rentals
            .Select(b =>
                new RentalDto
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    UserId = b.UserId,
                    RentedAt = b.RentedAt,
                    RentalStatus = (RentalStatus)b.RentalStatusId
                })
            .ToListAsync();
    }
}
