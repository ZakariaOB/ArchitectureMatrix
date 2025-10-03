using Microsoft.EntityFrameworkCore;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Domain.Interfaces;
using OnionMachineMonitoring.Infrastructure.Persistence;
using System.Linq;

namespace OnionMachineMonitoring.Infrastructure.Repositories;

public class ProductionRepository : IProductionRepository
{
    private readonly MachineDbContext _db;
    public ProductionRepository(MachineDbContext db) => _db = db;

    public async Task<IReadOnlyList<MachineProduction>> GetByMachineAndRangeAsync(
        int machineId, 
        DateTime fromUtc, 
        DateTime toUtc, 
        CancellationToken ct)
    {
        var rows = await _db.Productions.AsNoTracking()
            .Where(p => p.MachineId == machineId && p.TimestampUtc >= fromUtc && p.TimestampUtc < toUtc)
            .OrderBy(p => p.TimestampUtc)
            .ToListAsync(ct);

        return rows;
    }

    public async Task UpsertRangeAsync(IEnumerable<MachineProduction> items, CancellationToken ct)
    {
        foreach (var d in items)
        {
            var row = await _db.Productions.FirstOrDefaultAsync(
                p => p.MachineId == d.MachineId && p.TimestampUtc == d.TimestampUtc, ct);
            /*
            if (row is null)
            {
                _db.Productions.Add(new MachineProduction
                (
                    TimestampUtc = d.TimestampUtc,
                    Amount = d.Amount
                );
            }
            else
            {
                row.Amount = d.Amount;
            }*/
        }
        await _db.SaveChangesAsync(ct);
    }

    /*
    private static MachineProduction MapToDomain(MachineProduction p)
        => new(p.MachineId, DateTime.SpecifyKind(p.TimestampUtc, DateTimeKind.Utc), p.Amount);*/
}
