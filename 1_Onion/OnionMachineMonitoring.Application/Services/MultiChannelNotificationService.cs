using MachineMonitoring.Application;
using MachineMonitoring.Domain;

namespace MachineMonitoring.Infrastructure;

/// <summary>
/// Outbound fan-out (ServiceBus + Audit + Email). Works in Onion,
/// but delivery policies start living here and bleeding into the app over time.
/// </summary>
public sealed class MultiChannelNotificationService : INotificationService
{
    private readonly bool _sendServiceBus;
    private readonly bool _writeAudit;
    private readonly bool _sendEmail;

    public MultiChannelNotificationService(bool sendServiceBus = true, bool writeAudit = true, bool sendEmail = false)
    {
        _sendServiceBus = sendServiceBus; // HEX-JUSTIFY: per-target toggles/policies should be adapter-level
        _writeAudit = writeAudit;
        _sendEmail = sendEmail;
    }

    public Task PublishProdSyncedAsync(int count, object range, CancellationToken ct)
    {
        // HEX-JUSTIFY: different idempotency/retry/payload needs per channel
        if (_sendServiceBus)
        {
            // TODO: publish to bus
        }
        if (_writeAudit)
        {
            // TODO: write audit record
        }
        if (_sendEmail)
        {
            // TODO: send email summary
        }
        return Task.CompletedTask;
    }

    public Task PublishProdSyncedAsync(int count, object range)
    {
        throw new NotImplementedException();
    }
}
