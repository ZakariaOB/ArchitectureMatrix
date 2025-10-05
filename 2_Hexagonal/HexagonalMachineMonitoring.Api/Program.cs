using ArchitectureMatrix.HexagonalMachineMonitoring.Core.Ports.Inbound;
using ArchitectureMatrix.HexagonalMachineMonitoring.Adapters.Composition;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register hexagonal ports/adapters using config
// builder.Services.AddHexagonalProductionSync(builder.Configuration);

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

app.Run();
