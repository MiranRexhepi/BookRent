using BookRental.Data;
using BookRental.DTOs;
using BookRental.Enums;
using BookRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Queries;

public class GetTenantAvailableBooks(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<PaginatedResponse<BookDto>> Execute(int tenantId, Pagination pagination, BookFilters filters)
    {
        var query = _context.Books
            .Where(x => x.TenantId == tenantId
                        && x.BookStatus.Id == (int)BookStatus.Available
                        && !x.IsDeleted
                        && (string.IsNullOrEmpty(filters.Title) || x.Title == filters.Title)
                        && (string.IsNullOrEmpty(filters.Genre) || x.Genre == filters.Genre)
                        && (string.IsNullOrEmpty(filters.Author) || x.Author == filters.Author)
                        && (string.IsNullOrEmpty(pagination.Search) || x.Title.ToUpper().Contains(pagination.Search.ToUpper().Trim()) || x.ISBN.ToUpper().Contains(pagination.Search.ToUpper().Trim()))
            );

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
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

        return new PaginatedResponse<BookDto>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}