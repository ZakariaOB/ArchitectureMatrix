using HexagonalMachineMonitoring.Core.Domain.Models;
using HexagonalMachineMonitoring.Core.Domain.Models.Rules;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;

namespace HexagonalMachineMonitoring.Core.Services;

public class TemperatureMonitoringService(
    IGetTemperaturePort source,
    ISendAlertPort alert,
    IStoreTemperaturePort store,
    double threshold = 50.0)
{
    private readonly IGetTemperaturePort _source = source;
    private readonly ISendAlertPort _alert = alert;
    private readonly IStoreTemperaturePort _store = store;
    private readonly double _threshold = threshold;

    public async Task<TemperatureReading> MonitorAsync(int machineId)
    {
        var reading = await _source.GetLatestAsync(machineId);

        await _store.SaveAsync(reading);

        if (TemperatureExceededRule.IsExceeded(reading, _threshold))
        {
            await _alert.SendAsync(machineId, reading.Value);
        }

        return reading;
    }
}
