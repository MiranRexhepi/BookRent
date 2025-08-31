using BookRental.Constants;
using BookRental.Data;
using BookRental.Data.Entities;
using BookRental.Models;
using Microsoft.EntityFrameworkCore;

namespace BookRental.Features.Books.Queries;

public class GetBooksBySelectedColumn(BookRentalContext context)
{
    private readonly BookRentalContext _context = context;

    public async Task<PaginatedResponse<string>> Execute(int tenantId, string column, Pagination pagination)
    {
        if (string.IsNullOrEmpty(column))
            throw new InvalidOperationException(Messages.ColumnMustBeSpecified);

        var query = _context.Books
            .Where(x => x.TenantId == tenantId
                        && x.BookStatus.Id == (int)Enums.BookStatus.Available
                        && !x.IsDeleted
            );

        Func<IQueryable<Book>, IQueryable<string>> selectColumn = column switch
        {
            BookColumns.Title => q => q.Select(x => x.Title),
            BookColumns.Author => q => q.Select(x => x.Author),
            BookColumns.Genre => q => q.Select(x => x.Genre),
            _ => _ => throw new InvalidOperationException(Messages.ColumnMustBeSpecified)
        };

        int totalItems = await selectColumn(query)
            .Distinct()
            .CountAsync();

        var itemsQuery = selectColumn(query)
            .Distinct()
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .AsEnumerable();

        return new PaginatedResponse<string>
        {
            Items = itemsQuery,
            TotalItems = totalItems,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
}