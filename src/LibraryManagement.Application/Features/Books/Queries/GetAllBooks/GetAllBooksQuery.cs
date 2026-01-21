using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks;

public record GetAllBooksQuery : IRequest<IEnumerable<BookResponse>>;
