using Microsoft.AspNetCore.Mvc;
using OnionMachineMonitoring.Application.Services;

namespace OnionMachineMonitoring.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MachineController : ControllerBase
{
    private readonly MachineService _machineService;

    public MachineController(MachineService machineService)
    {
        _machineService = machineService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var machines = await _machineService.GetAllAsync();
        return Ok(machines);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var machine = await _machineService.GetByIdAsync(id);
        if (machine is null)
            return NotFound();
        return Ok(machine);
    }

    [HttpGet("{id}/total-production")]
    public async Task<IActionResult> GetTotalProductionAsync(int id)
    {
        var total = await _machineService.GetByIdAsync(id);
        return Ok(new { MachineId = id, TotalProduction = total });
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync([FromBody] CreateMachineRequest request)
    {
        await _machineService.AddAsync(request.Id, request.Name, request.Description);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        await _machineService.DeleteAsync(id);
        return Ok();
    }
}
