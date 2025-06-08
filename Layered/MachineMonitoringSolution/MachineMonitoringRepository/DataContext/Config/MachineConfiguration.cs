using MachineMonitoringRepository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MachineMonitoring.Repository.DataContext.Config
{
    internal class MachineConfiguration : IEntityTypeConfiguration<Machine>
    {
        public void Configure(EntityTypeBuilder<Machine> builder)
        {
            builder.ToTable("Machine");

            builder.Property(e => e.MachineId).HasColumnName("machineId");

            builder.Property(e => e.Description)
                .HasMaxLength(250)
                .IsUnicode(false);

            builder.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.HasMany(e => e.MachineProductions)
                .WithOne()
                .HasForeignKey(e => e.MachineId);
        }
    }
}
