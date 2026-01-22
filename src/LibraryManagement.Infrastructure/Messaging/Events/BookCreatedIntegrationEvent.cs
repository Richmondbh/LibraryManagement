using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Messaging.Events
{
    public record BookCreatedIntegrationEvent
    {
        public Guid BookId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Author { get; init; } = string.Empty;
        public string ISBN { get; init; } = string.Empty;
        public int PublishedYear { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
