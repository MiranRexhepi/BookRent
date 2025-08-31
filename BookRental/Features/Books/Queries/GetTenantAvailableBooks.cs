using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Queries;

public class GetTenantAvailableBooks(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<IEnumerable<BookDto>> Execute(int tenantId)
    {
        return await _context.Books
            .Where(x => x.TenantId == tenantId && x.BookStatus.Id == (int)BookStatus.Available && !x.IsDeleted)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Genre = b.Genre,
                ISBN = b.ISBN,
                TenantId = b.TenantId
            })
            .ToListAsync();
    }
}