using MachineMonitoring.WebAPI.Test.Constant;
using MachineMonitoringWebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MachineMonitoring.WebAPI.Test.IntegrationTests
{
    public class MachineMonitoringTestFixture : IDisposable
    {
        public HttpClient Client { get; }

        public MachineMonitoringTestFixture()
        {
            
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Dictionary<string, string> configurationValues = new()
            {
                { Constants.MachineMonitoringConnectionKey, config[Constants.MachineMonitoringConnectionKey] },
                { Constants.IncludeStaticFilesKey         , config[Constants.IncludeStaticFilesKey] }
            };

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configurationValues)
                .Build();

            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder
                        .UseConfiguration(configuration)
                        .ConfigureAppConfiguration(configurationBuilder =>
                        {
                            configurationBuilder.AddInMemoryCollection(configurationValues);
                        });
                });
            Client = webApplicationFactory.CreateDefaultClient();
        }

        public void Dispose() => Client.Dispose();
    }
}
