// Infrastructure/Persistence/MachineMonitoringDbContext.cs
using Microsoft.EntityFrameworkCore;
using OnionMachineMonitoring.Core.Entities;

namespace OnionMachineMonitoring.Infrastructure.Persistence;

public class MachineDbContext : DbContext
{
    public MachineDbContext(DbContextOptions<MachineDbContext> options)
        : base(options) { }

    public DbSet<Machine> Machines { get; set; }
    public DbSet<MachineProduction> Productions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MachineProduction>()
            .HasOne(p => p.Machine)
            .WithMany(m => m.Productions)
            .HasForeignKey(p => p.MachineId);

        base.OnModelCreating(modelBuilder);
    }
}
