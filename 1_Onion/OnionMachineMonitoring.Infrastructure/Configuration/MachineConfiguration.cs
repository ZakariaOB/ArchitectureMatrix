using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnionMachineMonitoring.Core.Entities;

namespace OnionMachineMonitoring.Infrastructure.Configurations;

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.ToTable("Machines");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Name).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Description).HasMaxLength(255);

        builder.HasMany(m => m.Productions)
               .WithOne(p => p.Machine)
               .HasForeignKey(p => p.MachineId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.Metadata
               .FindNavigation(nameof(Machine.Productions))!
               .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
