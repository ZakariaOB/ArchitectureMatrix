using MachineMonitoringClean10.UseCases.Machines.Delete;
using MachineMonitoringClean10.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Delete Machine endpoint.
/// Clean Architecture: Deletion rules are enforced in the domain, not here.
/// </summary>
public class Delete(IMediator mediator)
  : Endpoint<DeleteMachineRequest,
          Results<NoContent,
                  NotFound,
                  ProblemHttpResult>>
{
  public override void Configure()
  {
    Delete(DeleteMachineRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Delete a machine";
      s.Description = "Deletes a machine. Machine must be inactive/decommissioned and have no recent production.";
      s.Responses[204] = "Machine deleted successfully";
      s.Responses[404] = "Machine not found";
      s.Responses[400] = "Machine cannot be deleted (active or has recent production)";
    });
    Tags("Machines");
  }

  public override async Task<Results<NoContent, NotFound, ProblemHttpResult>>
    ExecuteAsync(DeleteMachineRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new DeleteMachineCommand(request.MachineId));
    return result.ToDeleteResult();
  }
}

public class DeleteMachineRequest
{
  public const string Route = "/Machines/{MachineId:int}";
  public int MachineId { get; set; }
}
