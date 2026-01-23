using LibraryManagement.Application.Common.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public class UpdateBookCommandHandler: IRequestHandler<UpdateBookCommand, bool>
    {
        private readonly IBookRepository _bookRepository;
        private readonly ICacheService _cacheService;
        private readonly IMessagePublisher _messagePublisher;
        public UpdateBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService, IMessagePublisher messagePublisher)
        {
            _bookRepository = bookRepository;
            _cacheService = cacheService;
            _messagePublisher = messagePublisher;
        }

        public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);

            if (book is null)
                return false;

            book.Update(
                request.Title,
                request.Author,
                request.ISBN,
                request.PublishedYear
            );

            await _bookRepository.UpdateAsync(book, cancellationToken);

            // Invalidate caches
            await _cacheService.RemoveAsync($"books:{request.Id}", cancellationToken);
            await _cacheService.RemoveAsync("books:all", cancellationToken);

            // Publish integration event
            await _messagePublisher.PublishAsync(new
            {
                BookId = book.Id,
                book.Title,
                book.Author,
                book.ISBN,
                book.PublishedYear,
                UpdatedAt = DateTime.UtcNow,
                OccurredAt = DateTime.UtcNow
            }, "book-updated", cancellationToken);
            return true;
        }
    }
}
