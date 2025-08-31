using BookRental.Data;
using BookRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Commands;

public class ReturnBookCommand(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task Execute(int tenantId, int bookId, string userId, int rentalId)
    {
        var bookIsAvailableForReturn = await _context.Books
            .Include(x => x.BookStatus)
            .AnyAsync(x => x.Id == bookId
                           && x.TenantId == tenantId
                           && !x.IsDeleted
                           && x.BookStatusId != (int)Enums.BookStatus.Available);

        if (!bookIsAvailableForReturn)
            throw new InvalidOperationException("Book is not available for rent");

        await _context.Books
            .Where(x => x.Id == bookId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.BookStatusId, (int)Enums.BookStatus.Available));

        await _context.Rentals
            .Where(x => x.Id == rentalId && x.UserId == userId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.RentalStatusId, (int)Enums.RentalStatus.Returned)
                    .SetProperty(y => y.ReturnedAt, DateTime.UtcNow));

        var rentalLog = new RentalLog
        {
            RentalId = rentalId,
            RentalStatusId = (int)Enums.RentalStatus.Returned
        };

        await _context.RentalLogs.AddAsync(rentalLog);

        await _context.SaveChangesAsync();
    }
}
