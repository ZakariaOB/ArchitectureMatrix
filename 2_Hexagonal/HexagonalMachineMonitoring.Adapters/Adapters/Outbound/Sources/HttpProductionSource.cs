using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sources;

public sealed class HttpProductionSource : IProductionSource
{
    public Task<IReadOnlyList<MachineProduction>> LoadAsync(string machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
        => Task.FromResult<IReadOnlyList<MachineProduction>>(new[]
        {
            new MachineProduction(machineId, fromUtc.AddHours(2), fromUtc.AddHours(3), 150)
        });
}
