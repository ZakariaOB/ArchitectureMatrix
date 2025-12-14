using MachineMonitoringClean10.Core.ContributorAggregate;

namespace MachineMonitoringClean10.UseCases.Contributors.Update;

public record UpdateContributorCommand(ContributorId ContributorId, ContributorName NewName) : ICommand<Result<ContributorDto>>;
