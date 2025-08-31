using BookRental.Constants;
using BookRental.Data;
using BookRental.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Commands;

public class UpdateBookCommand(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task Execute(int id, int tenantId, UpdateBookDto dto)
    {
        await _context.Books
            .Where(x => x.Id == id && x.TenantId == tenantId && !x.IsDeleted)
            .ExecuteUpdateAsync(x => x
                    .SetProperty(y => y.Title, dto.Title)
                    .SetProperty(y => y.Author, dto.Author)
                    .SetProperty(y => y.Genre, dto.Genre)
                    .SetProperty(y => y.ISBN, dto.ISBN));

        var res = await _context.SaveChangesAsync();

        if (res < 1)
            throw new KeyNotFoundException(Messages.BookNotFound);
    }
}
