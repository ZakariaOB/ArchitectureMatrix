using MachineMonitoring.Shared.Enums;

namespace MachineMonitoringRepository.Repositories.BaseRepository
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<EntityDeleteResult> Delete(int id);
    }
}
