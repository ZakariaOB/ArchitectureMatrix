using MachineMonitoring.Shared.Enums;
using MachineMonitoringService.Dto;

namespace MachineMonitoringService.Services
{
    public interface IMachineService
    {
        Task<IEnumerable<MachineDto>> GetAllAsync();

        Task<MachineDto> GetByIdAsync(int id);

        Task<EntityDeleteResult> Delete(int id);

        Task<EntityDeleteResult> DeleteIfAllowedAsync(int id);
    }
}
