using AutoMapper;
using MachineMonitoringService.Config;
using MachineMonitoringService.Services;
using MachineMonitoringWebAPI.Config.Profiles;
using MachineMonitoringWebAPI.Controllers;
using Moq;

namespace MachineMonitoring.WebAPI.Test.UnitTests
{
    public class UnitTestBase
    {
        protected readonly Mock<IMachineService> _machineServiceMock;
        protected readonly IMapper _mapper;
        protected readonly MachineApiController _controller;

        public UnitTestBase()
        {
            _machineServiceMock = new Mock<IMachineService>();
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.AddProfile(new MachineProfile());
                config.AddProfile(new MachineApiProfile());
            });
            _mapper = mappingConfig.CreateMapper();
            _controller = new MachineApiController(_machineServiceMock.Object, _mapper);
        }
    }
}
