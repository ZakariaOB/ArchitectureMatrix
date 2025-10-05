using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Domain.Interfaces;

namespace MachineMonitoring.Infrastructure;

/// <summary>
/// Good Onion trick: aggregate N sources behind the single repo abstraction,
/// then delegate Upsert to the sink repo. Works, but becomes glue over time.
/// </summary>
public sealed class CompositeProductionRepository : IProductionRepository
{
    private readonly IReadOnlyList<IProductionRepository> _sources; // SQL/HTTP/CSV as “sources”
    private readonly IProductionRepository _sink;                   // Cosmos as “sink”

    // HEX-JUSTIFY: per-source toggles and telemetry creeping in
    private readonly bool _enableSql;
    private readonly bool _enableHttp;
    private readonly bool _enableCsv;

    public CompositeProductionRepository(
        IEnumerable<IProductionRepository> sources,
        IProductionRepository sink,
        bool enableSql = true,
        bool enableHttp = true,
        bool enableCsv = true)
    {
        _sources = sources.ToList();
        _sink = sink;
        _enableSql = enableSql;   // HEX-JUSTIFY: rollout toggles belong at adapter/port wiring
        _enableHttp = enableHttp;  // HEX-JUSTIFY: env-based selection, SLA, rate limits…
        _enableCsv = enableCsv;   // HEX-JUSTIFY: source-specific policies
    }

    public async Task<IReadOnlyList<MachineProduction>> FetchAsync(
        object range, 
        CancellationToken ct)
    {
        var selected = _sources.Where((s, i) =>
        {
            // naive type checks to mimic per-source routing
            if (s is SqlProductionRepository) return _enableSql;
            if (s is HttpProductionRepository) return _enableHttp;
            if (s is CsvProductionRepository) return _enableCsv;
            return true;
        }).ToList();

        // HEX-JUSTIFY: composite starts owning source-specific pagination/join rules
        var batches = await Task.WhenAll(selected.Select(s => s.FetchAsync(range)));
        var merged = batches.SelectMany(x => x).ToList();

        // HEX-JUSTIFY: any per-source remediation (e.g., trim/UTC) tends to leak here
        return merged;
    }

    public Task<IReadOnlyList<MachineProduction>> FetchAsync(object range)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyList<MachineProduction>> GetByMachineAndRangeAsync(int machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task UpsertAsync(IReadOnlyList<MachineProduction> items)
        => _sink.UpsertAsync(items);

    public Task UpsertRangeAsync(IEnumerable<MachineProduction> items)
    {
        throw new NotImplementedException();
    }
}
