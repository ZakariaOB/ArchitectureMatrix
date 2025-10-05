namespace ArchitectureMatrix.HexagonalMachineMonitoring.Core.Domain;

public sealed class MachineProduction
{
    public string MachineId { get; }
    public DateTime StartUtc { get; }
    public DateTime EndUtc { get; }
    public int Quantity { get; }

    public MachineProduction(string machineId, DateTime startUtc, DateTime endUtc, int quantity)
    {
        MachineId = machineId ?? throw new ArgumentNullException(nameof(machineId));
        StartUtc = startUtc;
        EndUtc = endUtc;
        Quantity = quantity;
    }
}
