using SensorRegistry.Services.Interfaces;

namespace SensorRegistry.BgServices
{
    public class SensorStatusWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SensorStatusWorker> _logger;

        public SensorStatusWorker(IServiceProvider serviceProvider, ILogger<SensorStatusWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<ISensorService>();
                        _logger.LogInformation("[SensorRegistry] Starting check for faulty sensors");

                        try
                        {
                            await service.DeactivateFaultySensors();
                            _logger.LogInformation("[SensorRegistry] Checked for faulty sensors");

                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[SensorRegistry] Error occurred while checking faulty sensors");
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
