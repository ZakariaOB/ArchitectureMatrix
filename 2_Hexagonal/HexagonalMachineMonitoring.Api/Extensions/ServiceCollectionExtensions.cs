using Adapters.Decorators;
using Adapters.Outbound;
using Application.Ports.Outbound;
using HexagonalMachineMonitoring.Adapters.Adapters.Decorators;
using HexagonalMachineMonitoring.Adapters.Adapters.Inbound;
using HexagonalMachineMonitoring.Adapters.Adapters.Outbound;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Services;
using HexagonalMachineMonitoring.Core.UseCases;

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

    public static IServiceCollection AddMachineHealthMonitoring(this IServiceCollection services)
    {
        // Outbound ports
        services.AddSingleton<IStoreTelemetryPort, SqlTelemetryStoreAdapter>();
        services.AddSingleton<IRunAnomalyDetectionPort>(_ => new SimpleAnomalyDetectionAdapter(threshold: 80.0));
        services.AddSingleton<IPublishEventPort, ServiceBusEventAdapter>();

        // Notification with decorators (logging + retry)
        services.AddSingleton<ISendNotificationPort>(sp =>
        {
            ISendNotificationPort inner = new EmailNotificationAdapter();
            inner = new LoggingNotificationDecorator(inner);
            inner = new RetryNotificationDecorator(inner, retries: 2);
            return inner;
        });

        // Inbound use case
        services.AddSingleton<IIngestTelemetryUseCase, IngestTelemetryUseCase>();

        return services;
    }
}