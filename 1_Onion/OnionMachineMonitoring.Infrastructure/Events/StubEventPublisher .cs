using Microsoft.Extensions.Logging;
using OnionMachineMonitoring.Application.Interfaces;

namespace OnionMachineMonitoring.Infrastructure.Events
{
    internal sealed class StubEventPublisher : IEventPublisher
    {
        private readonly ILogger<StubEventPublisher> _logger;
        public StubEventPublisher(ILogger<StubEventPublisher> logger) => _logger = logger;

        public Task PublishAsync<T>(T @event, CancellationToken ct)
        {
            _logger.LogInformation("Sync event published (stub): {EventType}", typeof(T).FullName);
            return Task.CompletedTask;
        }
    }
}
