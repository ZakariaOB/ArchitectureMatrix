using AutoMapper;
using MachineMonitoring.Shared.Enums;
using MachineMonitoring.Shared.Extensions;
using MachineMonitoring.WebAPI.Models;
using MachineMonitoringService.Dto;
using MachineMonitoringService.Services;
using MachineMonitoringWebAPI.Constants;
using MachineMonitoringWebAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace MachineMonitoringWebAPI.Controllers
{
    [ApiController]
    public class MachineApiController : ControllerBase
    {
        private readonly IMachineService _machineService;

        private readonly IMapper _mapper;

        public MachineApiController(IMachineService machineService, IMapper mapper)
        {
            _machineService = machineService;
            _mapper = mapper;
        }

        [HttpGet(ApiRoutes.Machines.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            IEnumerable<MachineDto> machineDtos = await _machineService.GetAllAsync();
            if (machineDtos.IsNullOrEmpty())
            {
                return NoContent();
            }

            IEnumerable<MachineWithTotalProductionModel> machineModels = 
                _mapper.Map<IEnumerable<MachineWithTotalProductionModel>>(machineDtos);

            return Ok(machineModels);
        }

        [HttpGet(ApiRoutes.Machines.Get)]
        public async Task<IActionResult> GetByIdAsync([FromRoute] int id)
        {
            MachineDto machineDto = await _machineService.GetByIdAsync(id);
            if (machineDto == null)
            {
                return NotFound(null);
            }

            MachineModel machineModel = _mapper.Map<MachineModel>(machineDto);
            
            return Ok(machineModel);
        }

        [HttpGet(ApiRoutes.Machines.GetTotalProduction)]
        public async Task<IActionResult> GetMachineTotalProductionAsync([FromQuery] int id)
        {
            MachineDto machineDto = await _machineService.GetByIdAsync(id);
            if (machineDto == null)
            {
                return NotFound(null);
            }

            MachineTotalProductionResult machineTotalProductionResult = 
                _mapper.Map<MachineTotalProductionResult>(machineDto);
            
            return Ok(machineTotalProductionResult);
        }

        [HttpDelete(ApiRoutes.Machines.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            EntityDeleteResult deletionResult = await _machineService.Delete(id);
            if (deletionResult == EntityDeleteResult.Deleted)
            {
                return Ok((int)deletionResult);
            }
            return NotFound((int)deletionResult);
        }

        [HttpGet(ApiRoutes.Machines.GetForDashboard)]
        public async Task<IActionResult> GetForDashboardIdAsync([FromRoute] int id)
        {
            MachineDto machineDto = await _machineService.GetByIdAsync(id);
            if (machineDto == null)
            {
                return NotFound(null);
            }
            MachineForDashboardModel machineModel = _mapper.Map<MachineForDashboardModel>(machineDto);
            return Ok(machineModel);
        }
    }
}
