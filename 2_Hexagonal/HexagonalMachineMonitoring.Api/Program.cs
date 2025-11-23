using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Adapters.Adapters.Inbound;
using HexagonalMachineMonitoring.Adapters.Adapters.Outbound;
using HexagonalMachineMonitoring.Api.Extensions;
using HexagonalMachineMonitoring.Core.Ports.Inbound;
using HexagonalMachineMonitoring.Core.Ports.Outbound;
using HexagonalMachineMonitoring.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMachineMonitoring();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/hex/sync", async (ISyncProductionUseCase useCase, string machineId, DateTime fromUtc, DateTime toUtc, CancellationToken ct) =>
{
    var count = await useCase.SyncAsync(machineId, fromUtc, toUtc, ct);
    return Results.Ok(new { machineId, fromUtc, toUtc, synced = count });
})
.WithName("SyncProduction")
.WithOpenApi();


app.MapGet("/monitor/{machineId}", async (
    int machineId,
    TemperatureMonitoringService service) =>
{
    var reading = await service.MonitorAsync(machineId);
    return Results.Ok(reading);
});

app.Run();
