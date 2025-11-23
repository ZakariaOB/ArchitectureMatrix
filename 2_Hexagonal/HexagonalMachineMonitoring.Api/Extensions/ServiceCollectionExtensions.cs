using HexagonalMachineMonitoring.Adapters.Adapters.Inbound;
using HexagonalMachineMonitoring.Adapters.Adapters.Outbound;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Services;

namespace HexagonalMachineMonitoring.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMachineMonitoring(this IServiceCollection services)
    {
        // -----------------------------
        // Inbound Adapters (Drivers)
        // -----------------------------
        // Example: Temperature source from Sensor (default)
        services.AddSingleton<IGetTemperaturePort, SensorMqttAdapter>();

        // -----------------------------
        // Outbound Adapters (Driven)
        // -----------------------------
        // Storage
        services.AddSingleton<IStoreTemperaturePort, FileStorageAdapter>();

        // Alerts (wrapped with decorators if needed)
        services.AddSingleton<ISendAlertPort>(provider =>
        {
            // Base adapter
            ISendAlertPort inner = new EmailAlertAdapter();

            // Optional decorators
            // inner = new LoggingAlertAdapter(inner);
            // inner = new RetryAlertAdapter(inner);

            return inner;
        });

        // -----------------------------
        // Application Services
        // -----------------------------
        services.AddSingleton<TemperatureMonitoringService>();

        return services;
    }
}