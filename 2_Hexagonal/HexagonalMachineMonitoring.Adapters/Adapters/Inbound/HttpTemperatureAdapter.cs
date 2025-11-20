using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Inbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Inbound;

public class HttpTemperatureAdapter : IGetTemperaturePort
{
    public Task<TemperatureReading> GetLatestAsync(int id)
        => Task.FromResult(
            new TemperatureReading(id, 44.3, DateTime.UtcNow));
}
