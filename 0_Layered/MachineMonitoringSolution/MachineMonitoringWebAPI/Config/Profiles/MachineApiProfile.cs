using AutoMapper;
using MachineMonitoring.WebAPI.Models;
using MachineMonitoringService.Dto;
using MachineMonitoringWebAPI.Models;

namespace MachineMonitoringWebAPI.Config.Profiles
{
    public class MachineApiProfile : Profile
    {
        public MachineApiProfile()
        {
            CreateMap<MachineModel, MachineDto>().ReverseMap();

            CreateMap<MachineDto, MachineModelBase>()
                .ForMember(target => target.MachineId, opt => opt.MapFrom(src => src.MachineId))
                .ForMember(target => target.Name, opt => opt.MapFrom(src => src.Name));

            CreateMap<MachineDto, MachineModel>()
                .IncludeBase<MachineDto, MachineModelBase>()
                .ForMember(target => target.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<MachineDto, MachineWithTotalProductionModel>()
                .IncludeBase<MachineDto, MachineModelBase>()
                .ForMember(target => target.Production, opt => opt.MapFrom(src => src.MachineProductions.Sum(machine => machine.TotalProduction)));
            
            CreateMap<MachineDto, MachineForDashboardModel>()
                .IncludeBase<MachineDto, MachineModelBase>()
                .ForMember(target => target.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(target => target.Production, opt => opt.MapFrom(src => src.MachineProductions.Sum(machine => machine.TotalProduction)));

            CreateMap<MachineDto, MachineTotalProductionResult>()
                .ForMember(target => target.TotalProduction, opt => opt.MapFrom(src => src.MachineProductions.Sum(machine => machine.TotalProduction)));
        }
    }
}
