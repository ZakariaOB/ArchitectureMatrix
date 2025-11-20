using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Adapters.Adapters.Outbound;

public class FileStorageAdapter : IStoreTemperaturePort
{
    public Task SaveAsync(TemperatureReading reading)
    {
        Console.WriteLine($"[STORE] Saved: {reading.Value} at {reading.Timestamp}");
        return Task.CompletedTask;
    }
}
