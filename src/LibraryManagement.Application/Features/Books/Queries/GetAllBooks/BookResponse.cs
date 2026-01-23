using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Application.Features.Books.Queries.GetAllBooks;

public record BookResponse
(
  Guid Id,
    string Title,
    string Author,
    string ISBN,
    int PublishedYear,
    string? CoverImageUrl


);