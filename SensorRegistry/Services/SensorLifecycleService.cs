using MediatR;
using SensorRegistry.Features.Commands;
using SensorRegistry.Services.Interfaces;

namespace SensorRegistry.Services
{
    public class SensorLifecycleService : ISensorLifecycleService
    {
        private readonly IMediator _mediator;
        private readonly IK8Service _k8sService;
        private readonly ILogger<SensorLifecycleService> _logger;

        public SensorLifecycleService(IMediator mediator, IK8Service k8sService, ILogger<SensorLifecycleService> logger)
        {
            _mediator = mediator;
            _k8sService = k8sService;
            _logger = logger;
        }

        public async Task DeactivateSensorsAsync(List<Guid> ids, CancellationToken token)
        {
            await _mediator.Send(new SetSensorStatusCommand(ids, false), token);

            foreach (Guid id in ids)
            {
                try
                {
                    await _k8sService.ShutDownAndStartPod(id);
                    _logger.LogInformation($"[SensorRegistry] Successfully shut down pod for sensor: {id}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"[SensorRegistry]Failed to shut down pod for sensor: {id}. Continuing to next...");
                }
            }
            
        }
    }
}
