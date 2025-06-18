using MachineMonitoring.Repository.DataContext;
using MachineMonitoringRepository.Repositories;
using MachineMonitoringService.Services;
using Microsoft.EntityFrameworkCore;

namespace MachineMonitoringWebAPI.Config
{
    public static class ServiceInstaller
    {
        public static void ConfigureServices(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MachineMonitoringContext>(options => options.UseSqlServer(connectionString));

            services.AddScoped<IMachineService, MachineService>();

            services.AddScoped<IMachineRepository, MachineRepository>();
        }
    }
}
