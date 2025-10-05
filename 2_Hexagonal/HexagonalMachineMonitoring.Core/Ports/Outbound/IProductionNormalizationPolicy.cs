using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

/// <summary>Single source of truth for normalization rules.</summary>
public interface IProductionNormalizationPolicy
{
    IReadOnlyList<MachineProduction> Normalize(IEnumerable<MachineProduction> rows);
}
