using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Core.Ports.Inbound;
public interface IGetTemperaturePort
{
    Task<TemperatureReading> GetLatestAsync(int machineId);
}
