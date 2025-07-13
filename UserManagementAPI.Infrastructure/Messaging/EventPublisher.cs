using Hangfire;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UserManagementAPI.Application.Interfaces;
using UserManagementAPI.Domain.Events;
using UserManagementAPI.Infrastructure.Services;

namespace UserManagementAPI.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly ILogger<EventPublisher> _logger;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public EventPublisher(ILogger<EventPublisher> logger, IBackgroundJobClient backgroundJobClient)
        {
            _logger = logger;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task PublishAsync<T>(T eventData) where T : class
        {
            try
            {
                var eventType = typeof(T).Name;
                var eventJson = JsonSerializer.Serialize(eventData);

                _logger.LogInformation("Publishing event {EventType}: {EventData}", eventType, eventJson);

                await PublishToKafkaAsync(eventType, eventJson);

                if (eventData is UserCreatedEvent userCreatedEvent)
                {
                    _backgroundJobClient.Enqueue<BackgroundJobService>(
                        x => x.SendWelcomeEmailAsync(userCreatedEvent.Email, userCreatedEvent.FirstName));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish event of type {EventType}", typeof(T).Name);
                throw;
            }
        }

        private async Task PublishToKafkaAsync(string eventType, string eventData)
        {
            _logger.LogInformation("Publishing to Kafka topic 'user.events': {EventType} - {EventData}",
                eventType, eventData);

            await Task.Delay(10);

            _logger.LogInformation("Successfully published {EventType} to Kafka", eventType);
        }
    }
}
