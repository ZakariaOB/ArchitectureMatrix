namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface IProductionEventPort
{
    Task PublishSyncedAsync(
        string machineId, 
        int count, 
        CancellationToken ct);
}
