using MachineMonitoringClean10.Core.ContributorAggregate;
using Vogen;

namespace MachineMonitoringClean10.Infrastructure.Data.Config;

[EfCoreConverter<ContributorId>]
[EfCoreConverter<ContributorName>]
internal partial class VogenEfCoreConverters;
