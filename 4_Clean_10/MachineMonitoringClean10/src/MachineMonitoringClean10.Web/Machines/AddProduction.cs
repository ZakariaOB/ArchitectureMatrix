using System.ComponentModel.DataAnnotations;
using FluentValidation;
using MachineMonitoringClean10.UseCases.Machines.AddProduction;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Add Production to a Machine endpoint.
/// Demonstrates Clean Architecture: Business rule (only active machines can record production)
/// is enforced in the domain entity, not in this endpoint.
/// </summary>
public class AddProduction(IMediator mediator)
  : Endpoint<AddProductionRequest,
          Results<Ok,
                  NotFound,
                  ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(AddProductionRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Record production for a machine";
      s.Description = "Records production output for an active machine. Machine must be in Active status.";
      s.ExampleRequest = new AddProductionRequest 
      { 
        MachineId = 1, 
        Quantity = 100, 
        ProducedAt = DateTime.UtcNow,
        BatchNumber = "BATCH-001"
      };
      s.Responses[200] = "Production recorded successfully";
      s.Responses[400] = "Machine is not active or invalid data";
      s.Responses[404] = "Machine not found";
    });
    Tags("Machines");
  }

  public override async Task<Results<Ok, NotFound, ProblemHttpResult>>
    ExecuteAsync(AddProductionRequest request, CancellationToken cancellationToken)
  {
    var command = new AddProductionCommand(
      request.MachineId,
      request.Quantity,
      request.ProducedAt,
      request.BatchNumber);

    var result = await mediator.Send(command);

    return result.Status switch
    {
      ResultStatus.Ok => TypedResults.Ok(),
      ResultStatus.NotFound => TypedResults.NotFound(),
      _ => TypedResults.Problem(
        title: "Add production failed",
        detail: string.Join("; ", result.Errors),
        statusCode: StatusCodes.Status400BadRequest)
    };
  }
}

public class AddProductionRequest
{
  public const string Route = "/Machines/{MachineId:int}/Production";
  
  public int MachineId { get; set; }
  
  [Required]
  public int Quantity { get; set; }
  
  [Required]
  public DateTime ProducedAt { get; set; }
  
  public string? BatchNumber { get; set; }
}

public class AddProductionValidator : Validator<AddProductionRequest>
{
  public AddProductionValidator()
  {
    RuleFor(x => x.Quantity)
      .GreaterThan(0)
      .WithMessage("Quantity must be positive.");
    
    RuleFor(x => x.ProducedAt)
      .NotEmpty()
      .LessThanOrEqualTo(DateTime.UtcNow)
      .WithMessage("Production date cannot be in the future.");
  }
}
