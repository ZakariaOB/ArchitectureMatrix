using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface IProductionSource
{
    Task<IReadOnlyList<MachineProduction>> LoadAsync(
        string machineId, 
        DateTime fromUtc, 
        DateTime toUtc, 
        CancellationToken ct);
}
