using AutoMapper;
using MachineMonitoringService.Config;
using MachineMonitoringWebAPI.Config.Profiles;

namespace MachineMonitoringWebAPI.Config
{
    public static class AutoMapperInstaller
    {
        public static void AddAutoMapperConfiguration(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new MachineProfile());
                config.AddProfile(new MachineApiProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
