namespace HexagonalMachineMonitoring.Core.Ports.Outbound;
public interface ISendAlertPort
{
    Task SendAsync(int machineId, double value);
}
