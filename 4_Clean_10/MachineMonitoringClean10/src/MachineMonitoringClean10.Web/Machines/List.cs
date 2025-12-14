using MachineMonitoringClean10.UseCases.Machines;
using MachineMonitoringClean10.UseCases.Machines.List;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// List all Machines endpoint.
/// </summary>
public class List(IMediator mediator) : EndpointWithoutRequest<MachineListResponse>
{
  public override void Configure()
  {
    Get("/Machines");
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "List all machines";
      s.Description = "Returns a paginated list of all machines with their current status and production totals.";
    });
    Tags("Machines");
  }

  public override async Task HandleAsync(CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new ListMachinesQuery(null, null));

    if (result.IsSuccess)
    {
      Response = new MachineListResponse(
        result.Value.Items.Select(m => new MachineRecord(
          m.Id,
          m.Name,
          m.Description,
          m.Status,
          m.CreatedAt,
          m.LastProductionAt,
          m.TotalProduction)).ToList(),
        result.Value.TotalCount);
    }
  }
}

public record MachineListResponse(List<MachineRecord> Machines, int TotalCount);
