using BookRental.Constants;
using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Commands;

public class CreateBookCommand(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<BookDto> Execute(CreateBookDto dto)
    {
        var tenantExists = await _context.Users
            .AnyAsync(u => u.TenantId == dto.TenantId);

        if (!tenantExists)
        {
            throw new InvalidOperationException(Messages.TenantNotFound);
        }

        var book = new Book
        {
            Title = dto.Title,
            Author = dto.Author,
            Genre = dto.Genre,
            ISBN = dto.ISBN,
            TenantId = dto.TenantId,
            BookStatusId = (int)Enums.BookStatus.Available
        };

        await _context.Books.AddAsync(book);

        await _context.SaveChangesAsync();

        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Genre = book.Genre,
            ISBN = book.ISBN,
            TenantId = book.TenantId
        };
    }
}
