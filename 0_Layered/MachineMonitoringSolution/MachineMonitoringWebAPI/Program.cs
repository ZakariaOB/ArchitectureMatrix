using MachineMonitoring.WebAPI.Middleware;
using MachineMonitoringWebAPI.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace MachineMonitoringWebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var serviceProvider = builder.Services.BuildServiceProvider();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var globalSettings = new GlobalSettings();
            builder.Configuration.GetSection(nameof(GlobalSettings)).Bind(globalSettings);

            // Add services
            builder.Services.ConfigureServices(globalSettings.MachineMonitoringConnection);

            // Add Automapper
            builder.Services.AddAutoMapperConfiguration();

            var app = builder.Build();
            
            app.UseRouting();
            if (globalSettings.IncludeStaticFiles)
            {
                // Using static files
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "ClientApp")),
                    RequestPath = "/ClientApp"
                });
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapControllers();

            app.UseMiddleware<ExceptionMiddleware>();

            app.Run();
        }
    }
}