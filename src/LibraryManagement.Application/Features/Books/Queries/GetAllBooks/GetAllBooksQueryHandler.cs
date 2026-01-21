using MediatR;
using LibraryManagement.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks;

public  class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, IEnumerable<BookResponse>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<IEnumerable<BookResponse>> Handle(
        GetAllBooksQuery request,
        CancellationToken cancellationToken)
    {
        var books = await _bookRepository.GetAllAsync(cancellationToken);

        return books.Select(book => new BookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.ISBN,
            book.PublishedYear
        ));
    }


}
