using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Messaging.Events
{
    public class BookDeletedIntegrationEvent
    {
        public Guid BookId { get; init; }
        public DateTime OccurredAt { get; init; } = DateTime.UtcNow;
    }
}
