namespace MachineMonitoringRepository.Models
{
    public partial class MachineProduction
    {
        public int MachineProductionId { get; set; }
        public int MachineId { get; set; }
        public int TotalProduction { get; set; }
    }
}
