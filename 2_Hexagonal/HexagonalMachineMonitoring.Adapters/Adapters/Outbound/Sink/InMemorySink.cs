using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sink;

/// <summary>Simple sink to demonstrate swapping (EF, File, etc.).</summary>
public sealed class InMemorySink : IProductionSink
{
    private static readonly List<MachineProduction> _store = new();
    public Task SaveAsync(IEnumerable<MachineProduction> rows, CancellationToken ct)
    {
        _store.AddRange(rows);
        return Task.CompletedTask;
    }

    // for demo / debugging
    public static IReadOnlyList<MachineProduction> Dump() => _store.ToList();
}
