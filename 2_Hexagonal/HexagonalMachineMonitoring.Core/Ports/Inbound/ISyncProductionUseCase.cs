namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Inbound;

public interface ISyncProductionUseCase
{
    Task<int> SyncAsync(
        string machineId, 
        DateTime fromUtc, 
        DateTime toUtc, 
        CancellationToken ct);
}
