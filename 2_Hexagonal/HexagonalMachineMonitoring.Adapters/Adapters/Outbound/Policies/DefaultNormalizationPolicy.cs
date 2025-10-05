using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Policies;

public sealed class DefaultNormalizationPolicy : IProductionNormalizationPolicy
{
    public IReadOnlyList<MachineProduction> Normalize(IEnumerable<MachineProduction> rows)
        => rows
            .Where(r => r.StartUtc < r.EndUtc)
            .Select(r => new MachineProduction(r.MachineId, r.StartUtc, r.EndUtc, Math.Max(0, r.Quantity)))
            .ToList();
}
