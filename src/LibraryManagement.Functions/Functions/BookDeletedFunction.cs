using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Functions.Functions;

public class BookDeletedFunction
{
    private readonly ILogger<BookDeletedFunction> _logger;

    public BookDeletedFunction(ILogger<BookDeletedFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(BookDeletedFunction))]
    public async Task Run(
        [ServiceBusTrigger("book-deleted", Connection = "ServiceBusConnection")]
        string messageBody)
    {
        _logger.LogInformation("🗑️ Book Deleted Event Received!");
        _logger.LogInformation("Message: {MessageBody}", messageBody);

        try
        {
            var bookEvent = JsonSerializer.Deserialize<BookDeletedEvent>(messageBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (bookEvent != null)
            {
                _logger.LogInformation("Book deleted: {BookId}", bookEvent.BookId);

                // TODO:  business logic here
                // - Remove from search index
                // - Clean up related files
                // - Archive data

                await Task.Delay(100); // Simulate work
                _logger.LogInformation("✅ Book deletion processed successfully");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing book deleted event");
            throw;
        }
    }
}

public record BookDeletedEvent
{
    public Guid BookId { get; init; }
    public DateTime OccurredAt { get; init; }
}

