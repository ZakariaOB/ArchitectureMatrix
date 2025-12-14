using MachineMonitoringClean10.Core.ContributorAggregate;

namespace MachineMonitoringClean10.UseCases.Contributors.Delete;

public record DeleteContributorCommand(ContributorId ContributorId) : ICommand<Result>;
