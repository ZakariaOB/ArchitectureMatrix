using Core.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using OnionMachineMonitoring.Core.Entities;

namespace App.Controllers;

[ApiController]
[Route("sync")]
public sealed class SyncController : ControllerBase
{
    private readonly SyncMachineProductionForDateRange _useCase;
    public SyncController(SyncMachineProductionForDateRange useCase) => _useCase = useCase;

    /// <summary>
    /// Accepts normalized domain-friendly items and triggers the sync.
    /// </summary>
    [HttpPost("{machineId:guid}")]
    public async Task<IActionResult> Post(Guid machineId, [FromBody] Request req, CancellationToken ct)
    {
        /*var items = req.Items.Select(i =>
            new MachineProduction(machineId, DateTime.SpecifyKind(i.TimestampUtc, DateTimeKind.Utc), i.Amount));*/

        MachineProduction[] items = [];

        await _useCase.ExecuteAsync(machineId, req.FromUtc, req.ToUtc, items, ct);

        return Accepted();
    }

    public sealed record Request(DateTime FromUtc, DateTime ToUtc, Item[] Items);
    public sealed record Item(DateTime TimestampUtc, decimal Amount);
}
