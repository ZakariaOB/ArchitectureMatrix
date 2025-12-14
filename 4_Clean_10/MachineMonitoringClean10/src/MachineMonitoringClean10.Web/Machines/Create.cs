using System.ComponentModel.DataAnnotations;
using MachineMonitoringClean10.Core.MachineAggregate;
using MachineMonitoringClean10.UseCases.Machines.Create;
using MachineMonitoringClean10.Web.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MachineMonitoringClean10.Web.Machines;

/// <summary>
/// Create a new Machine endpoint.
/// Demonstrates Clean Architecture's separation: Web layer only handles HTTP concerns.
/// </summary>
public class Create(IMediator mediator)
  : Endpoint<CreateMachineRequest,
          Results<Created<CreateMachineResponse>,
                  ValidationProblem,
                  ProblemHttpResult>>
{
  public override void Configure()
  {
    Post(CreateMachineRequest.Route);
    AllowAnonymous();
    Summary(s =>
    {
      s.Summary = "Create a new machine";
      s.Description = "Creates a new machine with the provided name and optional description.";
      s.ExampleRequest = new CreateMachineRequest { Name = "CNC Machine 001", Description = "Primary CNC machine" };
      s.ResponseExamples[201] = new CreateMachineResponse(1, "CNC Machine 001");
      s.Responses[201] = "Machine created successfully";
      s.Responses[400] = "Invalid input data - validation errors";
    });
    Tags("Machines");
  }

  public override async Task<Results<Created<CreateMachineResponse>, ValidationProblem, ProblemHttpResult>>
    ExecuteAsync(CreateMachineRequest request, CancellationToken cancellationToken)
  {
    var result = await mediator.Send(new CreateMachineCommand(request.Name!, request.Description));

    return result.ToCreatedResult(
      id => $"/Machines/{id.Value}",
      id => new CreateMachineResponse(id.Value, request.Name!));
  }
}

public class CreateMachineRequest
{
  public const string Route = "/Machines";

  [Required]
  public string? Name { get; set; }
  public string? Description { get; set; }
}

public class CreateMachineValidator : Validator<CreateMachineRequest>
{
  public CreateMachineValidator()
  {
    RuleFor(x => x.Name)
      .NotEmpty()
      .WithMessage("Machine name is required.")
      .MinimumLength(MachineName.MinLength)
      .MaximumLength(MachineName.MaxLength);
  }
}

public record CreateMachineResponse(int Id, string Name);
