using BookRental.Constants;
using BookRental.Data;
using BookRental.DTOs;
using BookRental.Features.Books.Commands;
using BookRental.Features.Books.Queries;
using BookRental.Helpers;
using BookRental.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookRental.Controllers;

[Route("api/books")]
[Authorize]
public class BooksController(BookRentalContext context) : ControllerBase
{
    private readonly BookRentalContext _context = context;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(
        [FromRoute] int id)
    {
        var query = new GetBookByIdQuery(_context);

        var book = await query.Execute(id);

        return Ok(book);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBooks(
        [FromQuery] Pagination pagination,
        [FromQuery] BookFilters filters)
    {
        var tenantId = User.GetTenantId();

        var query = new GetTenantAvailableBooks(_context);

        var books = await query.Execute(tenantId, pagination, filters);

        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateBookDto dto)
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var command = new CreateBookCommand(_context);

        var book = await command.Execute(dto, tenantId);

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        [FromRoute] int id,
        [FromBody] UpdateBookDto dto)
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var command = new UpdateBookCommand(_context);

        await command.Execute(id, tenantId, dto);

        return Ok(new { Message = Messages.BookUpdatedSuccessfully });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        [FromRoute] int id)
    {
        User.IsAdmin();

        var tenantId = User.GetTenantId();

        var command = new DeleteBookCommand(_context);

        await command.Execute(id, tenantId);

        return Ok(new { Message = Messages.BookDeletedSuccessfully });
    }

    [HttpGet("columns/{column}")]
    public async Task<IActionResult> GetBooksBySelectedColumn(
        [FromRoute] string column,
        [FromQuery] Pagination pagination)
    {
        var tenantId = User.GetTenantId();

        var query = new GetBooksBySelectedColumn(_context);

        var books = await query.Execute(tenantId, column, pagination);

        return Ok(books);
    }
}