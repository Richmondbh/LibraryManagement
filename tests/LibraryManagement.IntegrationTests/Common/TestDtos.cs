using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.IntegrationTests.Common;

public class TestDtos
{
    public record CreateBookRequest(
    string Title,
    string Author,
    string ISBN,
    int PublishedYear
);

    // Including Id in the update request if your API expects it
    public record UpdateBookRequest(
        Guid Id,
        string Title,
        string Author,
        string ISBN,
        int PublishedYear
    );

    public record BookResponse(
        Guid Id,
        string Title,
        string Author,
        string ISBN,
        int PublishedYear,
        string? CoverImageUrl
    );

    public record ErrorResponse(
        string Message,
        IDictionary<string, string[]>? Errors
    );
}
