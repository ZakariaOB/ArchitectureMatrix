using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Inbound;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.UseCases;

/// <summary>
/// Pull from all sources -> normalize via policy -> save -> publish event.
/// </summary>
public sealed class SyncProductionInteractor : ISyncProductionUseCase
{
    private readonly IEnumerable<IProductionSource> _sources;
    private readonly IProductionNormalizationPolicy _policy;
    private readonly IProductionSink _sink;
    private readonly IProductionEventPort _events;

    public SyncProductionInteractor(
        IEnumerable<IProductionSource> sources,
        IProductionNormalizationPolicy policy,
        IProductionSink sink,
        IProductionEventPort events)
    {
        _sources = sources;
        _policy = policy;
        _sink = sink;
        _events = events;
    }

    public async Task<int> SyncAsync(string machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        var all = new List<MachineProduction>();
        foreach (var s in _sources)
            all.AddRange(await s.LoadAsync(machineId, fromUtc, toUtc, ct));

        var normalized = _policy.Normalize(all);
        await _sink.SaveAsync(normalized, ct);
        await _events.PublishSyncedAsync(machineId, normalized.Count, ct);
        return normalized.Count;
    }
}
