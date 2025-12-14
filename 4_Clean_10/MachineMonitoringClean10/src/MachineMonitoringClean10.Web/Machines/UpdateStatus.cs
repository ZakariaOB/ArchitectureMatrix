using System.ComponentModel.DataAnnotations;
using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.UseCases.Machines.UpdateStatus;
using MachineMonitoringClean10.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Update Machine Status endpoint.
/// Demonstrates Clean Architecture: Status transition rules are in the domain entity.
/// </summary>
public class UpdateStatus(IMediator mediator)
  : Endpoint<UpdateMachineStatusRequest,
          Results<Ok,
                  NotFound,
                  ProblemHttpResult>>
{
  public override void Configure()
  {
    Put(UpdateMachineStatusRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Update machine status";
      s.Description = "Changes machine status. Valid transitions are enforced by the domain.";
      s.ExampleRequest = new UpdateMachineStatusRequest { MachineId = 1, Status = "Active" };
      s.Responses[200] = "Status updated successfully";
      s.Responses[400] = "Invalid status or transition not allowed";
      s.Responses[404] = "Machine not found";
    });
    Tags("Machines");
  }

  public override async Task<Results<Ok, NotFound, ProblemHttpResult>>
    ExecuteAsync(UpdateMachineStatusRequest request, CancellationToken cancellationToken)
  {
    var command = new UpdateMachineStatusCommand(request.MachineId, request.Status!);

    var result = await mediator.Send(command);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(),
      ResultStatus.NotFound => TypedResults.NotFound(),
      ResultStatus.Invalid => TypedResults.Problem(
        title: "Invalid status",
        detail: string.Join("; ", result.ValidationErrors.Select(e => e.ErrorMessage)),
        statusCode: StatusCodes.Status400BadRequest),
      _ => TypedResults.Problem(
        title: "Update status failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public class UpdateMachineStatusRequest
{
  public const string Route = "/Machines/{MachineId:int}/Status";
  
  public int MachineId { get; set; }
  
  [Required]
  public string? Status { get; set; }
}

public class UpdateMachineStatusValidator : Validator<UpdateMachineStatusRequest>
{
  public UpdateMachineStatusValidator()
  {
    RuleFor(x => x.Status)
      .NotEmpty()
      .WithMessage("Status is required.")
      .Must(status => MachineStatus.TryFromName(status, ignoreCase: true, out _))
      .WithMessage("Invalid status. Valid values: Active, Inactive, Maintenance, Decommissioned");
  }
}
