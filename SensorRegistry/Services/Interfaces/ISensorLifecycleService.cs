namespace SensorRegistry.Services.Interfaces
{
    public interface ISensorLifecycleService
    {
        Task DeactivateSensorsAsync(List<Guid> ids, CancellationToken token);
    }
}
