using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Inbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Inbound;

public class SensorMqttAdapter : IGetTemperaturePort
{
    public Task<TemperatureReading> GetLatestAsync(int id)
        => Task.FromResult(
            new TemperatureReading(id, 45.0, DateTime.UtcNow));
}
