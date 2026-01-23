using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Functions.Functions;

public class BookCreatedFunction
{
    private readonly ILogger<BookCreatedFunction> _logger;

    public BookCreatedFunction(ILogger<BookCreatedFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(BookCreatedFunction))]
    public async Task Run(
        [ServiceBusTrigger("book-created", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        _logger.LogInformation("📚 Book Created Event Received!");
        _logger.LogInformation("Message: {MessageBody}", messageBody);

        try
        {
            var bookEvent = JsonSerializer.Deserialize<BookCreatedEvent>(messageBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (bookEvent != null)
            {
                _logger.LogInformation(
                    "Processing new book: {Title} by {Author} (ISBN: {ISBN})",
                    bookEvent.Title,
                    bookEvent.Author,
                    bookEvent.ISBN);

                // TODO:  business logic here
                // - Send welcome email
                // - Update search index
                // - Notify subscribers
                // - Generate thumbnail
                // - etc.

                await SimulateSendNotificationAsync(bookEvent);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing book created event");
            throw; // Rethrow to trigger retry/dead-letter
        }
    }

    private async Task SimulateSendNotificationAsync(BookCreatedEvent book)
    {
        // Simulate async work
        await Task.Delay(100);

        _logger.LogInformation(
            "✅ Notification sent for book: {Title}",
            book.Title);
    }
}

public record BookCreatedEvent
{
    public Guid BookId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string ISBN { get; init; } = string.Empty;
    public int PublishedYear { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime OccurredAt { get; init; }
}
