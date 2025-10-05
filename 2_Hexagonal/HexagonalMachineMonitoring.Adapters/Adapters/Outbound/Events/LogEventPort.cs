using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Events;

public sealed class LogEventPort : IProductionEventPort
{
    public Task PublishSyncedAsync(string machineId, int count, CancellationToken ct)
    {
        Console.WriteLine($"[SyncDone] {machineId} -> {count}");
        return Task.CompletedTask;
    }
}