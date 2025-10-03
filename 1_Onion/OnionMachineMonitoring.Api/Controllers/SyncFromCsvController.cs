// App/Controllers/SyncFromCsvController.cs  (CSV upload — DUPLICATES parsing)
using Core.Application.UseCases;
using Microsoft.AspNetCore.Mvc;
using OnionMachineMonitoring.Core.Entities;

namespace App.Controllers;

[ApiController, Route("sync-csv")]
public sealed class SyncFromCsvController : ControllerBase
{
    private readonly SyncMachineProductionForDateRange _useCase;
    public SyncFromCsvController(SyncMachineProductionForDateRange useCase)
        => _useCase = useCase;

    [HttpPost("{machineId:guid}")]
    public async Task<IActionResult> Post(
        Guid machineId, 
        [FromQuery] DateTime fromUtc, 
        [FromQuery] DateTime toUtc, 
        IFormFile file, 
        CancellationToken ct)
    {
        if (file is null || file.Length == 0) 
            return BadRequest("CSV required.");

        // DUPLICATED parsing/normalization #1
        var items = new List<MachineProduction>();
        using var reader = new StreamReader(file.OpenReadStream());
        var _ = await reader.ReadLineAsync(); // header
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            if (parts.Length < 2) continue;
            if (!DateTime.TryParse(parts[0], out var ts)) continue;
            if (!decimal.TryParse(parts[1], out var amt)) continue;

            var tsUtc = DateTime.SpecifyKind(ts, DateTimeKind.Utc);
            if (tsUtc >= fromUtc && tsUtc < toUtc)
            {
                // items.Add(new MachineProduction(machineId, tsUtc, amt));
            }
        }

        await _useCase.ExecuteAsync(machineId, fromUtc, toUtc, items, ct);
        return Accepted(new { synced = items.Count });
    }
}
