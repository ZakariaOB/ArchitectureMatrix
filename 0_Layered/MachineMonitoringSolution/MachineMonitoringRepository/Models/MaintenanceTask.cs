public class MaintenanceTask
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid MachineId { get; set; }
    public string Failure { get; set; } = string.Empty;
    public bool IsClosed { get; set; }
    public Guid? RescheduledFrom { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
