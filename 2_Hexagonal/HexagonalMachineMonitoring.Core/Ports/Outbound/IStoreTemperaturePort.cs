using HexagonalMachineMonitoring.Core.Domain.Models;

namespace HexagonalMachineMonitoring.Core.Ports.Outbound;


public interface IStoreTemperaturePort
{
    Task SaveAsync(TemperatureReading reading);
}
