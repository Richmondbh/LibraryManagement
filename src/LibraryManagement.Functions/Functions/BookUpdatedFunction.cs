using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Functions.Functions;

public class BookUpdatedFunction
{
    private readonly ILogger<BookUpdatedFunction> _logger;

    public BookUpdatedFunction(ILogger<BookUpdatedFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(BookUpdatedFunction))]
    public async Task Run(
        [ServiceBusTrigger("book-updated", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        _logger.LogInformation("📝 Book Updated Event Received!");
        _logger.LogInformation("Message: {MessageBody}", messageBody);

        try
        {
            var bookEvent = JsonSerializer.Deserialize<BookUpdatedEvent>(messageBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (bookEvent != null)
            {
                _logger.LogInformation(
                    "Book updated: {Title} (ID: {BookId})",
                    bookEvent.Title,
                    bookEvent.BookId);

                // TODO: business logic here
                // - Update search index
                // - Invalidate CDN cache
                // - Notify subscribers of changes

                await Task.Delay(100); // Simulate work
                _logger.LogInformation("✅ Book update processed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing book updated event");
            throw;
        }
    }
}

public record BookUpdatedEvent
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string ISBN { get; init; } = string.Empty;
    public int PublishedYear { get; init; }
    public DateTime UpdatedAt { get; init; }
    public DateTime OccurredAt { get; init; }
}

