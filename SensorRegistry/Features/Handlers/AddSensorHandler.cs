using MediatR;
using SensorRegistry.Data;
using SensorRegistry.Features.Commands;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Handlers
{
    public class AddSensorHandler : IRequestHandler<AddSensorCommand, bool>
    {
        private readonly SensorDbContext _context;
        private readonly ILogger<AddSensorHandler> _logger;

        public AddSensorHandler(SensorDbContext context, ILogger<AddSensorHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(AddSensorCommand command, CancellationToken token) 
        {
            if (await _context.Sensors.FindAsync(command.Data.Id) != null)
            {
                throw new InvalidOperationException($"[SensorRegistry] Sensor ID already exists");
            }

            Sensor sensor = new Sensor
            {
                Id = command.Data.Id,
                Quality = command.Data.Quality,
                PublicKey = command.Data.PublicKey,
                IsActive = true,
                IsMalicious = false,
                DateTimeOfRegistration = DateTime.UtcNow,
                PodName = command.Data.PodName
            };

            try
            {
                await _context.Sensors.AddAsync(sensor);
                _logger.LogInformation($"[SensorRegistry] Added new sensor to registry");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[SensorRegistry] Error occurred while adding new sensors");
                return false;
            }


            return await _context.SaveChangesAsync() > 0;
        }
    }
}
