using BookRental.Constants;
using BookRental.Data;
using BookRental.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Queries;

public class GetBookByIdQuery(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<BookDto?> Execute(int id)
    {
        return await _context.Books
            .Where(b => b.Id == id && !b.IsDeleted)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                ISBN = b.ISBN,
                TenantId = b.TenantId
            })
            .FirstOrDefaultAsync() ?? throw new KeyNotFoundException(Messages.BookNotFound);
    }
}
