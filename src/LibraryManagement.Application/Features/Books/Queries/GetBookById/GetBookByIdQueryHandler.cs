using LibraryManagement.Application.Common.Interfaces;
using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookById;

public class GetBookByIdQueryHandler: IRequestHandler<GetBookByIdQuery, BookResponse?>
{
    private readonly IBookRepository _bookRepository;
    public GetBookByIdQueryHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<BookResponse?> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _bookRepository.GetByIdAsync(request.Id, cancellationToken);

        if (book is null)
            return null;

        return new BookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.ISBN,
            book.PublishedYear,
            book.CoverImageUrl
        );
    }
}
