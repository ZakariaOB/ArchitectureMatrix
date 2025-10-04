using OnionMachineMonitoring.Domain.Interfaces;

namespace MachineMonitoring.Application;

/// <summary>
/// Good Onion use case: domain-pure, depends inward on abstractions only.
/// </summary>
public sealed class SyncProductionService
{
    private readonly IProductionRepository _repo;   // sources+sink hidden here
    private readonly INormalizationPolicy _policy; // domain policy
    private readonly INotificationService _notify; // outbound
    private readonly object _clock;  // testable time

    public SyncProductionService(
        IProductionRepository repo,
        INormalizationPolicy policy,
        INotificationService notify,
        object clock)
    {
        _repo = repo; _policy = policy; _notify = notify; _clock = clock;
    }

    public async Task ExecuteAsync(object range, CancellationToken ct = default)
    {
        // Fetch from “whatever is behind the repo” (could be SQL/HTTP/CSV)
        var fetched = await _repo.FetchAsync(range);

        // Normalize & merge in the core (OK in Onion)
        var merged = _policy.NormalizeAndMerge(fetched);

        // Persist to sink (wherever the repo writes)
        await _repo.UpsertAsync(merged);

        // Notify externally
        await _notify.PublishProdSyncedAsync(merged.Count, range);

        // _ = _clock.UtcNow; // prove time is injected
    }
}
