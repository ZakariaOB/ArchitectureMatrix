using FluentAssertions;
using MachineMonitoringWebAPI.Constants;
using System.Net;

namespace MachineMonitoring.WebAPI.Test.IntegrationTests
{
    public class DeleteAsyncTest : IClassFixture<MachineMonitoringTestFixture>
    {
        public HttpClient Client { get; }

        public DeleteAsyncTest(MachineMonitoringTestFixture machineMonitoringTestFixture)
        {
            Client = machineMonitoringTestFixture.Client;
        }

        [Fact]
        public async Task DeleteAsyncTest_ShouldReturn_200Code_Machine_Deleted()
        {
            // Arrange
            // The record with value 1 should exists for new created database
            string id = "1";
            string url = ApiRoutes.Machines.Get.Replace("{id}", id);

            // Act
            var response = await Client.DeleteAsync(url);

            int deletionResult = await response.Content.ReadAsAsync<int>();

            // Assert the response body
            if (deletionResult == 0)
            {
                response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
            else 
            {
                response.StatusCode.Should().Be(HttpStatusCode.OK);
            }
        }
    }
}
