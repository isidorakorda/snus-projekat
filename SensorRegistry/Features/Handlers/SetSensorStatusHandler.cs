using MediatR;
using Microsoft.EntityFrameworkCore;
using SensorRegistry.Data;
using SensorRegistry.Features.Commands;
using SensorRegistry.Models;
using SensorRegistry.Services.Interfaces;
using System.ComponentModel.Design;

namespace SensorRegistry.Features.Handlers
{
    public class SetSensorStatusHandler : IRequestHandler<SetSensorStatusCommand, bool>
    {
        private readonly SensorDbContext _context;
        private readonly ILogger<SetSensorStatusHandler> _logger;

        public SetSensorStatusHandler(SensorDbContext context, ILogger<SetSensorStatusHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(SetSensorStatusCommand command, CancellationToken token)
        {
            List<Sensor> sensors = await _context.Sensors.Where(s => command.Ids.Contains(s.Id)).ToListAsync(token);

            if (!sensors.Any()) return false;

            foreach (var sensor in sensors)
            {
                sensor.IsActive = command.IsActive;
                sensor.DeactivationTime = command.IsActive ? null : DateTime.UtcNow;
            }

            int count = await _context.SaveChangesAsync(token);

            _logger.LogInformation($"[SensorRegistry] Changed sensor status for {count}");

            return true;
        }
    }
}
