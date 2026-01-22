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
    public CreateBookCommandHandler(IBookRepository bookRepository, ICacheService cacheService)
    {
        _bookRepository = bookRepository;
        _cacheService = cacheService;
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

        return book.Id;
    }
}
