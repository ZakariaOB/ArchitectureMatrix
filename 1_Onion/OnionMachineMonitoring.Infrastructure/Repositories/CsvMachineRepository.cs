using OnionMachineMonitoring.Core.Entities;
using OnionMachineMonitoring.Core.Interfaces;

public class CsvMachineRepository : IMachineRepository
{
    private readonly string _path;

    public CsvMachineRepository(string path) => _path = path;

    public async Task<IEnumerable<Machine>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(_path, cancellationToken);
        return lines
            .Skip(1)
            .Select(line =>
            {
                var parts = line.Split(';');
                return new Machine(int.Parse(parts[0]), parts[1], parts.Length > 2 ? parts[2] : null);
            })
            .ToList();
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
