using Microsoft.Extensions.Logging;
using OnionMachineMonitoring.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
