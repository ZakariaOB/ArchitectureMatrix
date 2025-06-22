using Microsoft.EntityFrameworkCore;
using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;
using OnionMachineMonitoring.Infrastructure.Persistence;

namespace OnionMachineMonitoring.Infrastructure.Repositories;

public class MachineEfRepository : IMachineRepository
{
    private readonly MachineDbContext _context;

    public MachineEfRepository(MachineDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Machine>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Machines
            .Include(m => m.Productions)
            .ToListAsync(cancellationToken);
    }

    public async Task<Machine?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Machines
            .Include(m => m.Productions)
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task AddAsync(Machine machine, CancellationToken cancellationToken = default)
    {
        _context.Machines.Add(machine);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        Machine machine, 
        CancellationToken cancellationToken = default)
    {
        _context.Machines.Update(machine);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var machine = await _context.Machines.FindAsync([id], cancellationToken);
        if (machine is not null)
        {
            _context.Machines.Remove(machine);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
