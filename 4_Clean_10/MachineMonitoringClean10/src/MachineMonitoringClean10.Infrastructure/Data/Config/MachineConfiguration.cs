using MachineMonitoringClean10.Core.MachineAggregate;

namespace MachineMonitoringClean10.Infrastructure.Data.Config;

/// <summary>
/// EF Core configuration for Machine aggregate.
/// Clean Architecture: Infrastructure handles ORM mapping, domain stays pure.
/// </summary>
public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
  public void Configure(EntityTypeBuilder<Machine> builder)
  {
    builder.ToTable("Machines");

    // Configure strongly-typed ID with auto-generation
    builder.Property(entity => entity.Id)
      .HasValueGenerator<VogenIdValueGenerator<AppDbContext, Machine, MachineId>>()
      .HasVogenConversion()
      .IsRequired();

    // Configure MachineName value object
    builder.Property(entity => entity.Name)
      .HasVogenConversion()
      .HasMaxLength(MachineName.MaxLength)
      .IsRequired();

    builder.Property(entity => entity.Description)
      .HasMaxLength(500);

    // Configure MachineStatus SmartEnum
    builder.Property(x => x.Status)
      .HasConversion(
          x => x.Value,
          x => MachineStatus.FromValue(x))
      .IsRequired();

    builder.Property(entity => entity.CreatedAt)
      .IsRequired();

    builder.Property(entity => entity.LastProductionAt);

    // Configure owned collection of MachineProduction
    builder.OwnsMany(entity => entity.Productions, productionBuilder =>
    {
      productionBuilder.ToTable("MachineProductions");
      
      productionBuilder.Property(p => p.Quantity)
        .IsRequired();
      
      productionBuilder.Property(p => p.ProducedAt)
        .IsRequired();
      
      productionBuilder.Property(p => p.RecordedAt)
        .IsRequired();
      
      productionBuilder.Property(p => p.BatchNumber)
        .HasMaxLength(50);
    });
  }
}
