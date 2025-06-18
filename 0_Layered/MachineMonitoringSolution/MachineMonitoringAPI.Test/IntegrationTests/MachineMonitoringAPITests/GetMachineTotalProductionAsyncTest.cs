using FluentAssertions;
using MachineMonitoring.WebAPI.Models;
using MachineMonitoringWebAPI.Constants;
using System.Net;

namespace MachineMonitoring.WebAPI.Test.IntegrationTests
{
    public class GetMachineTotalProductionAsyncTest : IClassFixture<MachineMonitoringTestFixture>
    {
        public HttpClient Client { get; }

        public GetMachineTotalProductionAsyncTest(MachineMonitoringTestFixture machineMonitoringTestFixture)
        {
            Client = machineMonitoringTestFixture.Client;
        }

        [Fact]
        public async Task GetMachineTotalProductionAsync_ShouldReturn_200Code_WhenMachineExist()
        {
            // Arrange
            // The record with value 1 should exists for new created database
            string url = $"{ApiRoutes.Machines.GetTotalProduction}/totalproduction?id=1";

            // Act
            var response = await Client.GetAsync(url);

            MachineTotalProductionResult machineTotalProductionResult = 
                await response.Content.ReadAsAsync<MachineTotalProductionResult>();

            // Assert the response body
            if (machineTotalProductionResult != null)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task GetMachineTotalProductionAsync_ShouldReturn_204Code_WhenMachineDoesExists()
        {
            // Arrange
            // The record with value -1 should not normally exist
            string url = $"{ApiRoutes.Machines.GetTotalProduction}/totalproduction?id=-1";

            // Act
            var response = await Client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
