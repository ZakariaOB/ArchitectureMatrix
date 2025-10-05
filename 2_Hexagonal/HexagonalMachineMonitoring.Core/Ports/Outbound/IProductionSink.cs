using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

public interface IProductionSink
{
    Task SaveAsync(
        IEnumerable<MachineProduction> rows, 
        CancellationToken ct);
}
