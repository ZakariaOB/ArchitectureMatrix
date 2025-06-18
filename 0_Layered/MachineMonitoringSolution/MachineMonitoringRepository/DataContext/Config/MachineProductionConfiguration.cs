using MachineMonitoringRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MachineMonitoring.Repository.DataContext.Config
{
    internal class MachineProductionConfiguration : IEntityTypeConfiguration<MachineProduction>
    {
        public void Configure(EntityTypeBuilder<MachineProduction> builder)
        {
            builder.ToTable("MachineProduction");

            builder.Property(e => e.MachineId).HasColumnName("machineId");

            builder.Property(e => e.TotalProduction).HasColumnName("totalProduction");
        }
    }
}
