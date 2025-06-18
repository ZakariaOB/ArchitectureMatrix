namespace MachineMonitoringService.Dto
{
    public class MachineDto
    {
        public int MachineId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public virtual ICollection<MachineProductionDto> MachineProductions { get; set; }
    }
}
