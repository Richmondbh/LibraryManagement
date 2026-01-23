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
    private readonly ITelemetryService _telemetryService;
    public CreateBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService, IMessagePublisher messagePublisher, ITelemetryService telemetryService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
        _messagePublisher = messagePublisher;
        _telemetryService = telemetryService;
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

        // Track custom event in Application Insights
        _telemetryService.TrackEvent("BookCreated", new Dictionary<string, string>
        {
            { "BookId", book.Id.ToString() },
            { "Title", book.Title },
            { "Author", book.Author }
        });

        return book.Id;
    }
}
