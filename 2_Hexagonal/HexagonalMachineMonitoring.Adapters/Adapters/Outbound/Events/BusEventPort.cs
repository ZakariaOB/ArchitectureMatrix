using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Events;

public sealed class BusEventPort : IProductionEventPort
{
    public Task PublishSyncedAsync(string machineId, int count, CancellationToken ct)
    {
        // Here you'd push to Service Bus / Kafka.
        return Task.CompletedTask;
    }
}