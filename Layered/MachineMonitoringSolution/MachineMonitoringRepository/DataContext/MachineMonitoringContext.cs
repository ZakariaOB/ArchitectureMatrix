using MachineMonitoringRepository.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace MachineMonitoring.Repository.DataContext
{
    public partial class MachineMonitoringContext : DbContext
    {
        public MachineMonitoringContext(DbContextOptions<MachineMonitoringContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Machine> Machines { get; set; } = null!;
        public virtual DbSet<MachineProduction> MachineProductions { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                    Assembly.GetExecutingAssembly(),
                    t => t.GetInterfaces().Any(i =>
                            i.IsGenericType &&
                            i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
