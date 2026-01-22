using Azure.Messaging.ServiceBus;
using LibraryManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Messaging.ServiceBus
{
    public class ServiceBusPublisher : IMessagePublisher, IAsyncDisposable
    {
        private readonly ServiceBusClient _client;
        private readonly ILogger<ServiceBusPublisher> _logger;
        private readonly string _defaultQueueName;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ServiceBusPublisher(IConfiguration configuration, ILogger<ServiceBusPublisher> logger)
        {
            _logger = logger;

            var connectionString = configuration.GetConnectionString("ServiceBus");
            _defaultQueueName = configuration["ServiceBus:DefaultQueue"] ?? "book-events";

            if (string.IsNullOrEmpty(connectionString))
            {
                _logger.LogWarning("Service Bus connection string not configured. Messages will not be published.");
                _client = null!;
                return;
            }

            _client = new ServiceBusClient(connectionString);
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await PublishAsync(message, _defaultQueueName, cancellationToken);
        }

        public async Task PublishAsync<T>(T message, string queueOrTopicName, CancellationToken cancellationToken = default) where T : class
        {
            if (_client == null)
            {
                _logger.LogWarning("Service Bus not configured. Skipping message publish for {MessageType}", typeof(T).Name);
                return;
            }

            try
            {
                await using var sender = _client.CreateSender(queueOrTopicName);

                var messageBody = JsonSerializer.Serialize(message, JsonOptions);
                var serviceBusMessage = new ServiceBusMessage(messageBody)
                {
                    ContentType = "application/json",
                    MessageId = Guid.NewGuid().ToString(),
                    Subject = typeof(T).Name
                };

                await sender.SendMessageAsync(serviceBusMessage, cancellationToken);

                _logger.LogInformation(
                    "Published {MessageType} to {QueueOrTopic}: {MessageBody}",
                    typeof(T).Name,
                    queueOrTopicName,
                    messageBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish {MessageType} to {QueueOrTopic}", typeof(T).Name, queueOrTopicName);
                throw;
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_client != null)
            {
                await _client.DisposeAsync();
            }
        }
    }
}
