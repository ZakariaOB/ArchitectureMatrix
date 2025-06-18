using FluentAssertions;
using MachineMonitoringWebAPI.Constants;
using MachineMonitoringWebAPI.Models;
using System.Net;

namespace MachineMonitoring.WebAPI.Test.IntegrationTests
{
    public class GetByIdAsyncTest : IClassFixture<MachineMonitoringTestFixture>
    {
        public HttpClient Client { get; }

        public GetByIdAsyncTest(MachineMonitoringTestFixture machineMonitoringTestFixture)
        {
            Client = machineMonitoringTestFixture.Client;
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_200Code_WhenMachineExist()
        {
            // Arrange
            // The record with value 1 should exists for new created database
            string id = "1";
            string url = ApiRoutes.Machines.Get.Replace("{id}", id);

            // Act
            var response = await Client.GetAsync(url);

            MachineModel machineModel = await response.Content.ReadAsAsync<MachineModel>();

            // Assert the response body
            if (machineModel != null)
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturn_204Code_WhenMachineDoesExists()
        {
            // Arrange
            // The record with value -1 should not normally exist
            string id = "-1";
            string url = ApiRoutes.Machines.Get.Replace("{id}", id);

            // Act
            var response = await Client.GetAsync(url);

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
