using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook;


public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Guid>
{
    private readonly IBookRepository _bookRepository;
    private readonly ICacheService _cacheService;
    private readonly IMessagePublisher _messagePublisher;
    public CreateBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService, IMessagePublisher messagePublisher)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _messagePublisher = messagePublisher;
    }

    public async Task<Guid> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(
             request.Title,
             request.Author,
             request.ISBN,
             request.PublishedYear
         );

        await _bookRepository.AddAsync(book, cancellationToken);

        // Invalidate the "all books" cache
        await _cacheService.RemoveAsync("books:all", cancellationToken);

        // Publish integration event
        await _messagePublisher.PublishAsync(new
        {
            BookId = book.Id,
            book.Title,
            book.Author,
            book.ISBN,
            book.PublishedYear,
            book.CreatedAt,
            OccurredAt = DateTime.UtcNow
        }, "book-created", cancellationToken);


        return book.Id;
    }
}
