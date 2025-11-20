using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound
{
    public class SmsAlertAdapter : ISendAlertPort
    {
        public Task SendAsync(int machineId, double value)
        {
            Console.WriteLine($"[SMS] Machine {machineId} exceeded with {value}");
            return Task.CompletedTask;
        }
    }
}
