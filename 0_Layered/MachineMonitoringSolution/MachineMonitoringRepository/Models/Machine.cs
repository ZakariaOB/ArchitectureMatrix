namespace MachineMonitoringRepository.Models
{
    public partial class Machine
    {
        public Machine()
        {
            MachineProductions = new HashSet<MachineProduction>();
        }

        public int MachineId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; }

        public virtual ICollection<MachineProduction> MachineProductions { get; set; }
    }
}
