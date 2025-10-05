using System.Globalization;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sources;

/// <summary>ONLY parsing here. No normalization (policy owns it).</summary>
public sealed class CsvProductionSource : IProductionSource
{
    public Task<IReadOnlyList<MachineProduction>> LoadAsync(string machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        var lines = new[] { "2025-01-01T08:00Z,2025-01-01T09:00Z,200" };

        var rows = new List<MachineProduction>();
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            var start = DateTime.Parse(parts[0], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var end = DateTime.Parse(parts[1], CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
            var qty = int.Parse(parts[2], CultureInfo.InvariantCulture);
            rows.Add(new MachineProduction(machineId, start, end, qty));
        }
        return Task.FromResult<IReadOnlyList<MachineProduction>>(rows);
    }
}