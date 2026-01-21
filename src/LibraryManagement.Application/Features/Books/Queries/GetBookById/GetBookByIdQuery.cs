using LibraryManagement.Application.Features.Books.Queries.GetAllBooks;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Queries.GetBookById;

public record GetBookByIdQuery(Guid Id) : IRequest<BookResponse?>;
