using MachineMonitoring.Shared.Extensions;
using MachineMonitoring.WebAPI.Models;
using MachineMonitoringWebAPI.Constants;
using System.Net;

namespace MachineMonitoring.WebAPI.Test.IntegrationTests
{
    public class GetAllAsyncTest : IClassFixture<MachineMonitoringTestFixture>
    {
        public HttpClient Client { get; }

        public GetAllAsyncTest(MachineMonitoringTestFixture machineMonitoringTestFixture)
        {
            Client = machineMonitoringTestFixture.Client;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_200Code_WhenMachinesExist()
        {
            // Act
            var response = await Client.GetAsync(ApiRoutes.Machines.GetAll);

            IEnumerable<MachineWithTotalProductionModel> machineWithTotalProductionModels 
                = await response.Content.ReadAsAsync<IEnumerable<MachineWithTotalProductionModel>>();

            bool isMachinesDataNotEmpty = !machineWithTotalProductionModels.IsNullOrEmpty();

            Assert.Equal(response.StatusCode == HttpStatusCode.OK, isMachinesDataNotEmpty);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturn_204Code_WhenNoMachinesExist()
        {
            // Act
            var response = await Client.GetAsync(ApiRoutes.Machines.GetAll);

            IEnumerable<MachineWithTotalProductionModel> machineWithTotalProductionModels
                = await response.Content.ReadAsAsync<IEnumerable<MachineWithTotalProductionModel>>();

            bool isMachinesDataEmpty = machineWithTotalProductionModels.IsNullOrEmpty();

            Assert.Equal(response.StatusCode == HttpStatusCode.NoContent, isMachinesDataEmpty);
        }
    }
}
