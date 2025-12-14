using MachineMonitoringClean10.UseCases.Machines;
using MachineMonitoringClean10.UseCases.Machines.Get;
using MachineMonitoringClean10.Web.Extensions;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Get Machine by ID endpoint.
/// </summary>
public class GetById(IMediator mediator)
  : Endpoint<GetMachineByIdRequest,
          Results<Ok<MachineRecord>,
                  NotFound,
                  ProblemHttpResult>>
{
  public override void Configure()
  {
    Get(GetMachineByIdRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Get a machine by ID";
      s.Description = "Returns machine details including production statistics.";
      s.Responses[200] = "Machine found";
      s.Responses[404] = "Machine not found";
    });
    Tags("Machines");
  }

  public override async Task<Results<Ok<MachineRecord>, NotFound, ProblemHttpResult>>
    ExecuteAsync(GetMachineByIdRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new GetMachineQuery(request.MachineId));

    return result.ToGetByIdResult(dto => new MachineRecord(
      dto.Id,
      dto.Name,
      dto.Description,
      dto.Status,
      dto.CreatedAt,
      dto.LastProductionAt,
      dto.TotalProduction));
  }
}

public class GetMachineByIdRequest
{
  public const string Route = "/Machines/{MachineId:int}";
  public int MachineId { get; set; }
}
