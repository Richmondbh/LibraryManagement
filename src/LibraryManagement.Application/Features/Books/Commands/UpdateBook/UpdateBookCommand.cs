using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Commands.UpdateBook
{
    public record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    string ISBN,
    int PublishedYear
) : IRequest<bool>;
    
}
