using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;
using System.Net.Http.Json;

namespace OnionMachineMonitoring.Infrastructure.Repositories
{
    public class HttpMachineRepository : IMachineRepository
    {
        private readonly HttpClient _client;

        public HttpMachineRepository(HttpClient client) => _client = client;

        public async Task<IEnumerable<Machine>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _client.GetFromJsonAsync<List<Machine>>("/machines", cancellationToken);
            return result ?? [];
        }

        Task IMachineRepository.AddAsync(Machine machine, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IMachineRepository.DeleteAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<Machine>> IMachineRepository.GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<Machine> IMachineRepository.GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task IMachineRepository.UpdateAsync(Machine machine, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
