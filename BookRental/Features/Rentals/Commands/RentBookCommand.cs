using BookRental.Data;
using BookRental.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Rentals.Commands;

public class RentBookCommand(BookRentalContext context, Middleware.WebSocketManager wsManager)
{
    private readonly BookRentalContext _context = context;
    private readonly Middleware.WebSocketManager _wsManager = wsManager;

    public async Task Execute(int tenantId, int bookId, string userId)
    {
        var userIsNotAllowedToRent = await _context.Rentals
            .AnyAsync(x => x.UserId == userId
                           && !x.Book.IsDeleted
                           && x.RentalStatusId == (int)Enums.RentalStatus.Overdue);

        if (userIsNotAllowedToRent)
            throw new InvalidOperationException("You have books overdue");

        var overdueRentals = await _context.Rentals
            .Include(r => r.RentalLogs)
            .Where(x => x.UserId == userId
                        && !x.Book.IsDeleted
                        && x.RentalStatusId == (int)Enums.RentalStatus.Rented
                        && DateTime.UtcNow > x.RentedAt.AddDays(14))
            .ToListAsync();

        if (overdueRentals.Count != 0)
        {
            overdueRentals.ForEach(x =>
            {
                x.RentalStatusId = (int)Enums.RentalStatus.Overdue;

                x.RentalLogs.Add(
                    new RentalLog
                    {
                        RentalStatusId = (int)Enums.RentalStatus.Overdue
                    });
            });

            await _context.SaveChangesAsync();

            throw new InvalidOperationException("You have books overdue");
        }

        var exeededMaxNumberOfBooksAllowed = await _context.Rentals
            .CountAsync(x => x.UserId == userId
                           && !x.Book.IsDeleted
                           && x.RentalStatusId == (int)Enums.RentalStatus.Rented);

        if (exeededMaxNumberOfBooksAllowed > 2)
            throw new InvalidOperationException("You have reached maximum rent limit");

        var bookAvailableForRent = await _context.Books
            .AnyAsync(x => x.Id == bookId
                           && x.TenantId == tenantId
                           && !x.IsDeleted
                           && x.BookStatusId == (int)Enums.BookStatus.Available);

        if (!bookAvailableForRent)
            throw new InvalidOperationException("Book is not available for rent");

        await _context.Books
            .Where(x => x.Id == bookId)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.BookStatusId, (int)Enums.BookStatus.Rented));

        var rental = new Rental
        {
            BookId = bookId,
            RentedAt = DateTime.UtcNow,
            UserId = userId,
            RentalStatusId = (int)Enums.RentalStatus.Rented,
            RentalLogs =
            [
                new()
                    {
                        RentalStatusId = (int)Enums.RentalStatus.Rented
                    }
            ]
        };

        await _context.Rentals.AddAsync(rental);

        await _context.SaveChangesAsync();

        await _wsManager.BroadcastAsync($"Book rented: {bookId} by user {userId}");
    }
}

