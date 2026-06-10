using MediatR;
using SensorRegistry.DTOs;
using SensorRegistry.Features.Commands;
using SensorRegistry.Features.Handlers;
using SensorRegistry.Features.Queries;
using SensorRegistry.Models;
using SensorRegistry.Services;
using SensorRegistry.Services.Interfaces;

namespace SensorRegistry.BgServices
{
    public class SensorStatusWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SensorStatusWorker> _logger;

        public SensorStatusWorker(IServiceScopeFactory scopeFactory, ILogger<SensorStatusWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using (IServiceScope scope = _scopeFactory.CreateScope())
                    {
                        IMediator mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        ISensorLifecycleService sensorLifecycleService = scope.ServiceProvider.GetRequiredService<ISensorLifecycleService>();

                        _logger.LogInformation("[SensorRegistry] Starting check for faulty sensors");
                        DateTime timeout = DateTime.UtcNow.AddSeconds(-10);

                        List<SensorDTO> sensors = await mediator.Send(new GetAllSensorsQuery(true), stoppingToken);
                        List<Guid> faultyIds = await mediator.Send(new GetFaultySensorsQuery(sensors, timeout), stoppingToken);

                        if (faultyIds.Any())
                        {
                            await sensorLifecycleService.DeactivateSensorsAsync(faultyIds, stoppingToken);
                            _logger.LogInformation($"[SensorRegistry] Successfully processed {faultyIds.Count} faulty sensors");
                        }
                        else
                        {
                            _logger.LogInformation("[SensorRegistry] No faulty sensors found");
                        }
                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[SensorRegistry] Error in SensorStatusWorker");
                }
            }
        }
    }
}
