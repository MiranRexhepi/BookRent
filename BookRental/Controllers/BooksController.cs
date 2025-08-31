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
    public async Task<IActionResult> GetById(int id)
    {
        var query = new GetBookByIdQuery(_context);

        var book = await query.Execute(id);

        if (book == null)
            return NotFound();

        return Ok(book);
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailableBooks(
        [FromQuery] Pagination pagination,
        [FromQuery] BookFilters filters)
    {
        var tenantId = User.GetTenantId();

        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(Messages.MissingTenantId);

        if (!int.TryParse(tenantId, out var tenantIdInt))
            return BadRequest(Messages.InvalidTenantId);

        var query = new GetTenantAvailableBooks(_context);

        var books = await query.Execute(tenantIdInt, pagination, filters);

        if (books == null)
            return NotFound();

        return Ok(books);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateBookDto dto)
    {
        var role = User.GetRoleName();

        if (role != UserRoles.Admin)
            return Unauthorized(Messages.YouDontHavePermission);

        var command = new CreateBookCommand(_context);

        var book = await command.Execute(dto);

        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBookDto dto)
    {
        var role = User.GetRoleName();

        if (role != UserRoles.Admin)
            return Unauthorized(Messages.YouDontHavePermission);

        var tenantId = User.GetTenantId();

        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(Messages.MissingTenantId);

        if (!int.TryParse(tenantId, out var tenantIdInt))
            return BadRequest(Messages.InvalidTenantId);

        var command = new UpdateBookCommand(_context);

        var result = await command.Execute(id, tenantIdInt, dto);

        return result ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var role = User.GetRoleName();

        if (role != UserRoles.Admin)
            return Unauthorized(Messages.YouDontHavePermission);

        var tenantId = User.GetTenantId();

        if (string.IsNullOrEmpty(tenantId))
            return BadRequest(Messages.MissingTenantId);

        if (!int.TryParse(tenantId, out var tenantIdInt))
            return BadRequest(Messages.InvalidTenantId);

        var command = new DeleteBookCommand(_context);

        var result = await command.Execute(id, tenantIdInt);

        return result ? Ok(Messages.BookDeletedSuccessfully) : NotFound();
    }
}