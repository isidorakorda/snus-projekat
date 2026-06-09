using MediatR;
using Server.Data;
using Server.Features.Commands;
using Server.Model;

namespace Server.Features.Handlers
{
    public class IngestDataHandler : IRequestHandler<IngestDataCommand, bool>
    {
        private readonly ServerDbContext _context;
        private readonly ILogger<IngestDataHandler> _logger;

        public IngestDataHandler(ServerDbContext context, ILogger<IngestDataHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(IngestDataCommand command, CancellationToken token)
        {
            var dto = command.Data;

            var record = new SensorRecord
            {
                SensorId = dto.SensorId,
                Temperature = dto.Temperature,
                MessageId = dto.MessageId,
                Timestamp = dto.Timestamp,
                AlarmPriority = dto.AlarmPriority,
                IsConsensus = false
            };

            if (record.AlarmPriority > 0)
                LogAlarmInColor(record.SensorId, record.Temperature, record.AlarmPriority);

            try
            {
                await _context.SensorRecords.AddAsync(record, token);
                return await _context.SaveChangesAsync(token) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ Ingestion ] Error occured while saving measurements for sensors {dto.SensorId}");
            }
        }

        private void LogAlarmInColor(Guid sensorId, double temp, int priority)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = priority switch
            {
                1 => ConsoleColor.Yellow,
                2 => ConsoleColor.DarkYellow,
                3 => ConsoleColor.Red,
                _ => originalColor
            };

            Console.WriteLine($"[ ALARM - PRIORITY {priority}] : Sensor: {sensorId} | Temperature: {temp}");
            Console.ForegroundColor = originalColor;
        }
    }
}
