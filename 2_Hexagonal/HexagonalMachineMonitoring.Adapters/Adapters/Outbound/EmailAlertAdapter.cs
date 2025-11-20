using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound;

public class EmailAlertAdapter : ISendAlertPort
{
    public Task SendAsync(int machineId, double value)
    {
        Console.WriteLine($"[EMAIL] Machine {machineId} exceeded with {value}");
        return Task.CompletedTask;
    }
}
