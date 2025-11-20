using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Inbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Inbound;
public class SqlTemperatureAdapter : IGetTemperaturePort
{
    public Task<TemperatureReading> GetLatestAsync(int id)
        => Task.FromResult(
            new TemperatureReading(id, 42.1, DateTime.UtcNow));
}
