using ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Events;
using ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Policies;
using ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sink;
using ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Outbound.Sources;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Inbound;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Outbound;
using ArchitectureMatrix.HexagonalMachineMonitoring.Core.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Composition;

public static class HexagonalRegistration
{
    public static IServiceCollection AddHexagonalProductionSync(this IServiceCollection services, IConfiguration config)
    {
        // Sources selected via config array: "Ef", "Http", "Csv"
        var enabled = config.GetSection("Hex:Sources").Get<string[]>() ?? Array.Empty<string>();

        var sources = new List<IProductionSource>();
        if (enabled.Contains("Ef", StringComparer.OrdinalIgnoreCase)) sources.Add(new EfProductionSource());
        if (enabled.Contains("Http", StringComparer.OrdinalIgnoreCase)) sources.Add(new HttpProductionSource());
        if (enabled.Contains("Csv", StringComparer.OrdinalIgnoreCase)) sources.Add(new CsvProductionSource());

        services.AddSingleton<IEnumerable<IProductionSource>>(sources);
        services.AddSingleton<IProductionNormalizationPolicy, DefaultNormalizationPolicy>();
        services.AddSingleton<IProductionSink, InMemorySink>();

        // Swap this line to BusEventPort to demonstrate side-effect swapping.
        services.AddSingleton<IProductionEventPort, LogEventPort>();

        services.AddSingleton<ISyncProductionUseCase, SyncProductionInteractor>();
        return services;
    }
}
