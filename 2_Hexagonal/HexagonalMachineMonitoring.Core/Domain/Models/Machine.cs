namespace HexagonalMachineMonitoring.Core.Domain.Models;

public sealed class Machine
{
    public string Id { get; }
    public Machine(string id) => Id = id ?? throw new ArgumentNullException(nameof(id));
}
