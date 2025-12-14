using MachineMonitoringClean10.Core.ContributorAggregate;
using MachineMonitoringClean10.Core.MachineAggregate;
using Vogen;

namespace MachineMonitoringClean10.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
[EfCoreConverter<MachineId>]
[EfCoreConverter<MachineName>]
internal partial class VogenEfCoreConverters;
