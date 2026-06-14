using MediatR;
using SensorRegistry.Data;
using SensorRegistry.Features.Commands;
using SensorRegistry.Features.Queries;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Handlers
{
    public class GetSensorHandler : IRequestHandler<GetSensorQuery, string>
    {
        private readonly SensorDbContext _context;
        private readonly ILogger<GetSensorHandler> _logger;

        public GetSensorHandler(SensorDbContext context, ILogger<GetSensorHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> Handle(GetSensorQuery query, CancellationToken token)
        {
            Sensor? sensor = await _context.Sensors.FindAsync([query.SensorId], token);

            if (sensor == null)
            {
                _logger.LogWarning($"[SensorRegistry] Sensor with ID {query.SensorId} not found.");
                return null;
            }

            return sensor.PodName;
        }
    }
}
