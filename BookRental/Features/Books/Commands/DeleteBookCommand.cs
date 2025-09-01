using BookRental.Constants;
using BookRental.Data;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Commands;

public class DeleteBookCommand(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task Execute(int id, int tenantId)
    {
        var affectedRows = await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId && !x.IsDeleted)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(b => b.IsDeleted, true));

        await _context.SaveChangesAsync();

        if (affectedRows < 1)
            throw new KeyNotFoundException(Messages.BookNotFound);
    }
}