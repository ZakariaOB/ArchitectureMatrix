using HexagonalMachineMonitoring.Core.Ports.Outbound;
using System.Text.Json;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound;

public sealed class ServiceBusEventAdapter : IPublishEventPort
{
    public Task PublishAsync(string eventName, object payload, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(payload);
        Console.WriteLine($"[SERVICE BUS] Event '{eventName}' published: {json}");
        return Task.CompletedTask;
    }
}