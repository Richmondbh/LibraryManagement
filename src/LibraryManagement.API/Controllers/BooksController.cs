using LibraryManagement.Application.Features.Books.Commands.CreateBook;
using LibraryManagement.Application.Features.Books.Commands.DeleteBook;
using LibraryManagement.Application.Features.Books.Commands.UpdateBook;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using LibraryManagement.Application.Features.Books.Queries.GetBookById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LibraryManagement.API.Controllers
{
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
        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateBookCommand command, CancellationToken cancellationToken)
        {
            var bookId = await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = bookId }, bookId);
        }

        /// <summary>
        /// Delete a book
        /// </summary>
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
    }
}

