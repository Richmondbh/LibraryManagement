using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.CreateBook;

public  record CreateBookCommand(
    string Title,
    string Author,
    string ISBN,
    int PublishedYear
) : IRequest<Guid>;

