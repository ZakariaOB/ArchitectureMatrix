//------------------------------------------------------------------------------
// Use case: Upserts provided productions in [fromUtc,toUtc) for a machine,
// then emits a SyncCompleted event. No tech/policy logic here.
//------------------------------------------------------------------------------
using OnionMachineMonitoring.Application.Interfaces;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Domain.Interfaces;

namespace Core.Application.UseCases;

public sealed class SyncMachineProductionForDateRange
{
    private readonly IProductionRepository _repo;
    private readonly IEventPublisher _events;

    public SyncMachineProductionForDateRange(
        IProductionRepository repo, 
        IEventPublisher events)
    {
        _repo = repo;
        _events = events;
    }

    public async Task ExecuteAsync(Guid machineId, DateTime fromUtc, DateTime toUtc, IEnumerable<MachineProduction> incoming, CancellationToken ct)
    {
        await _repo.UpsertRangeAsync(incoming, ct);
        await _events.PublishAsync(new SyncCompleted(machineId, fromUtc, toUtc, DateTime.UtcNow), ct);
    }

    public sealed record SyncCompleted(Guid MachineId, DateTime FromUtc, DateTime ToUtc, DateTime CompletedAtUtc);
}
