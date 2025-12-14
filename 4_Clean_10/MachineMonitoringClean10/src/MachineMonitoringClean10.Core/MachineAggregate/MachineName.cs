using Vogen;

namespace MachineMonitoringClean10.Core.MachineAggregate;

/// <summary>
/// Value Object for Machine name with built-in validation.
/// Clean Architecture principle: Domain rules are encapsulated in the domain model.
/// </summary>
[ValueObject<string>(conversions: Conversions.SystemTextJson)]
public partial struct MachineName
{
  public const int MaxLength = 100;
  public const int MinLength = 2;

  private static Validation Validate(in string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      return Validation.Invalid("Machine name cannot be empty");
    
    if (name.Length < MinLength)
      return Validation.Invalid($"Machine name must be at least {MinLength} characters");
    
    if (name.Length > MaxLength)
      return Validation.Invalid($"Machine name cannot exceed {MaxLength} characters");
    
    return Validation.Ok;
  }
}
