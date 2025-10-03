namespace OnionMachineMonitoring.Application.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken ct);
    }
}
