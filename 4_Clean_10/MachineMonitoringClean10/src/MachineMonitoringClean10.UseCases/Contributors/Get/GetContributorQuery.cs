using MachineMonitoringClean10.Core.ContributorAggregate;

namespace MachineMonitoringClean10.UseCases.Contributors.Get;

public record GetContributorQuery(ContributorId ContributorId) : IQuery<Result<ContributorDto>>;
