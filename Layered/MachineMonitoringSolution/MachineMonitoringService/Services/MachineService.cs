using AutoMapper;
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

        public MachineService(IMachineRepository machineRepository, IMapper mapper) 
        {
            _machineRepository = machineRepository;
            _mapper  = mapper;
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
    }
}
