namespace OnionMachineMonitoring.Core.Entities;

public class Machine
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }

    private readonly List<MachineProduction> _productions = new();
    public IReadOnlyCollection<MachineProduction> Productions => _productions.AsReadOnly();

    // Required by EF Core
    private Machine() { }

    public Machine(int id, string name, string? description = null)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
    }

    public void AddProduction(int totalProduction, DateTime createdDate)
    {
        if (totalProduction < 0)
        {
            throw new ArgumentException("Production must be positive", nameof(totalProduction));
        }

        _productions.Add(new MachineProduction(this, totalProduction, createdDate));
    }

    public int GetTotalProduction() => _productions.Sum(p => p.TotalProduction);

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty", nameof(newName));

        Name = newName;
    }

    public void EnsureCanBeDeleted(DateTime now)
    {
        if (_productions.Any(p => p.CreatedAt >= now.AddDays(-30)))
            throw new DomainException("Machine has recent production within 30 days");
    }
}
