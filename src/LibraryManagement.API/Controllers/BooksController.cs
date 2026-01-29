using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.Features.Books.Commands.UploadBookCover;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.Application.Features.Books.Queries.GetBookById;
using LibraryManagement.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly IMediator _mediator;
    public BooksController(IMediator mediator) 
    { 
        _mediator = mediator;
    }

    /// <summary>
    /// Get all books from the database
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var books = await _mediator.Send(new GetAllBooksQuery(), cancellationToken);
        return Ok(books);
    }

    /// <summary>
    /// Get a book by its ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var book = await _mediator.Send(new GetBookByIdQuery(id), cancellationToken);

        if (book is null)
            return NotFound();

        return Ok(book);
    }

    /// <summary>
    /// Create a new book
    /// </summary>
    // [Authorize(Roles = Roles.Admin)]
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost]
    public async Task<ActionResult<Guid>> Create(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var bookId = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = bookId }, bookId);
    }

    /// <summary>
    /// Delete a book
    /// </summary>
    /// 
    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteBookCommand(id), cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // <summary>
    /// Update an existing book
    /// </summary>
    /// 
    [Authorize(Policy = "RequireAdminRole")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateBookCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(command, cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    /// <summary>
    /// Upload a cover image for a book
    /// </summary>
    /// 
    [Authorize(Policy = "RequireUserRole")]
    [HttpPost("{id:guid}/cover")]
    public async Task<IActionResult> UploadCover(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedTypes.Contains(file.ContentType.ToLower()))
            return BadRequest("Invalid file type. Allowed: JPEG, PNG, GIF, WebP");

        // Validate file size (max 5MB)
        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size exceeds 5MB limit");

        using var stream = file.OpenReadStream();

        var command = new UploadBookCoverCommand(
            id,
            stream,
            file.FileName,
            file.ContentType
        );

        var imageUrl = await _mediator.Send(command, cancellationToken);

        if (imageUrl == null)
            return NotFound("Book not found");

        return Ok(new { imageUrl });
    }

}

