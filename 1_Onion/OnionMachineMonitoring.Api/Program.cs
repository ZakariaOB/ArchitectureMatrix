using Microsoft.EntityFrameworkCore;
using OnionMachineMonitoring.Application.Interfaces;
using OnionMachineMonitoring.Application.Services;
using OnionMachineMonitoring.Core.Interfaces;
using OnionMachineMonitoring.Infrastructure.Persistence;
using OnionMachineMonitoring.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Register MachineRepository implementation based on config
var repoType = builder.Configuration["RepositoryType"]?.ToLowerInvariant();

switch (repoType)
{
    case "http":
        builder.Services.AddHttpClient<IMachineRepository, HttpMachineRepository>(client =>
        {
            client.BaseAddress = new Uri(builder.Configuration["HttpApiBaseUrl"]);
        });
        break;

    case "csv":
        builder.Services.AddScoped<IMachineRepository>(provider =>
        {
            var path = builder.Configuration["CsvPath"];
            return new CsvMachineRepository(path);
        });
        break;

    case "ef":
    default:
        builder.Services.AddDbContext<MachineDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
        builder.Services.AddScoped<IMachineRepository, MachineEfRepository>();
        break;
}

// Application services
builder.Services.AddScoped<IMachineService, MachineService>();

// API + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger();
    // app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
