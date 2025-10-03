using AutoMapper;
using MachineMonitoring.Repository.DataContext;
using MachineMonitoring.Shared.Enums;
using MachineMonitoringRepository.Models;
using MachineMonitoringRepository.Repositories;
using MachineMonitoringService.Dto;

namespace MachineMonitoringService.Services
{
    public class MachineService : IMachineService
    {
        IMachineRepository _machineRepository;

        readonly IMapper _mapper;

        MachineMonitoringContext machineMonitoringContext;

        public MachineService(
            IMachineRepository machineRepository,
            IMapper mapper,
            MachineMonitoringContext dbContext) 
        {
            _machineRepository = machineRepository;
            _mapper  = mapper;
            machineMonitoringContext = dbContext;
        }

        public async Task<IEnumerable<MachineDto>> GetAllAsync()
        {
            IEnumerable<Machine> machines = await _machineRepository.GetAllAsync();

            IEnumerable<MachineDto> machineDtos = _mapper.Map<IEnumerable<MachineDto>>(machines);

            return machineDtos;
        }

        public async Task<MachineDto> GetByIdAsync(int id)
        {
            Machine machine = await _machineRepository.GetByIdAsync(id);

            MachineDto machineDtos = _mapper.Map<MachineDto>(machine);

            return machineDtos;
        }

        public async Task<EntityDeleteResult> Delete(int id)
        {
            return await _machineRepository.Delete(id);
        }


        public async Task<EntityDeleteResult> DeleteIfAllowedAsync(int id)
        {
            var machine = await _machineRepository.GetByIdAsync(id);
            if (machine == null) return EntityDeleteResult.NotFound;

            if (!machine.CanBeDeleted(DateTime.UtcNow))
                return EntityDeleteResult.Forbidden;

            await _machineRepository.Delete(id);

            return EntityDeleteResult.Deleted;
        }
    }
}
