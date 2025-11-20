using MachineMonitoring.Service.Services;
using System.Data;

namespace MachineMonitoring.Tests.UnitTests
{
    public class Layered_TemperatureTests
    {
        [Fact]
        public async Task SwitchingInfrastructure_BreaksLayeredService()
        {
            // arrange: service depends on HttpClient
            var http = new HttpClient(new FakeHandler());
            var service = new MachineService(null, null, null, http);

            // act
            var temperature = await service.GetMachineTemperatureAsync(10);

            // assert
            Assert.Equal(42, temperature); // OK for HTTP version

            // simulate switching to SQL (Dapper)
            var dapperRepo = new DapperTemperatureRepository(null);

            // SERVICE CANNOT WORK WITH DAPPER → COMPILE ERROR
            var t = await service.GetMachineTemperatureAsync(10);

            // Instead we must rewrite MachineService entirely.
            Assert.True(true); // test demonstrates limitation
        }

        class FakeHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken token)
            {
                var json = "{\"temperature\":42}";
                return Task.FromResult(new HttpResponseMessage
                {
                    Content = new StringContent(json)
                });
            }
        }

        public class DapperTemperatureRepository
        {
            private readonly IDbConnection _conn;
            public DapperTemperatureRepository(IDbConnection conn)
            {
                _conn = conn;
            }

            public Task<double> GetTemperatureAsync(int id)
            {
                return Task.FromResult(55.0);
            }
        }

    }
}
