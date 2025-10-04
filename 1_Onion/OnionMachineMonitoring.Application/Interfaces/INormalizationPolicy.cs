using OnionMachineMonitoring.Core.Entities;

namespace MachineMonitoring.Application;

/// <summary>Central business normalization (dedupe, UTC coercion, mapping).</summary>
public interface INormalizationPolicy
{
    IReadOnlyList<MachineProduction> NormalizeAndMerge(IEnumerable<MachineProduction> items);
}
