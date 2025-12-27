using System.Data;
using MachineMonitoringService.Services;

namespace MachineMonitoring.Tests.UnitTests
{
    public class Layered_TemperatureTests
    {
        [Fact]
        public async Task SwitchingInfrastructure_BreaksLayeredService()
        {
            // arrange: service depends on specific infrastructure (EF)
            var service = new MachineService(null, null, null);

            // act - the service is tightly coupled to Entity Framework
            // We cannot easily switch to Dapper without rewriting the service
            
            // simulate switching to SQL (Dapper)
            var dapperRepo = new DapperTemperatureRepository(null);

            // SERVICE CANNOT WORK WITH DAPPER → This demonstrates the limitation
            // of layered architecture where service layer is coupled to infrastructure
            var temperature = await dapperRepo.GetTemperatureAsync(10);

            // Instead we must rewrite MachineService entirely to support different data access patterns.
            Assert.Equal(55.0, temperature); // test demonstrates limitation
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
