namespace OnionMachineMonitoring.Core.Entities;

public class MachineProduction
{
    public int Id { get; private set; }
    public int TotalProduction { get; private set; }

    public int MachineId { get; private set; }
    public Machine Machine { get; private set; }

    // Required by EF
    private MachineProduction() { }

    public MachineProduction(Machine machine, int totalProduction)
    {
        Machine = machine ?? throw new ArgumentNullException(nameof(machine));
        MachineId = machine.Id;
        TotalProduction = totalProduction > 0
            ? totalProduction
            : throw new ArgumentException("Production must be positive", nameof(totalProduction));
    }
}
