using AutoMapper;
using MachineMonitoringRepository.Models;
using MachineMonitoringService.Dto;

namespace MachineMonitoringService.Config
{
    public class MachineProfile : Profile
    {
        public MachineProfile()
        {
            CreateMap<Machine, MachineDto>().ReverseMap();

            CreateMap<MachineProduction, MachineProductionDto>().ReverseMap();
        }
    }
}
