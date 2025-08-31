using BookRental.Data;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Commands;

public class DeleteBookCommand(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<bool> Execute(int id, int tenantId)
    {
        await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId && !x.IsDeleted)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(b => b.IsDeleted, true));

        var res = await _context.SaveChangesAsync();

        return res > 0;
    }
}
