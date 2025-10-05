using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sources;

public sealed class EfProductionSource : IProductionSource
{
    public Task<IReadOnlyList<MachineProduction>> LoadAsync(string machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<MachineProduction>>(
        [
            new MachineProduction(machineId, fromUtc.AddHours(1), fromUtc.AddHours(2), 100)
        ]);
}
