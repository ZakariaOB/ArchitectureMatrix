using MachineMonitoringClean10.Core.ContributorAggregate;

namespace MachineMonitoringClean10.UseCases.Contributors;
public record ContributorDto(ContributorId Id, ContributorName Name, PhoneNumber PhoneNumber);
