using MediatR;
using Server.Data;
using Server.Features.Commands;
using Server.Model;
using Server.Service.IService;

namespace Server.Features.Handlers
{
    public class IngestDataHandler : IRequestHandler<IngestDataCommand, bool>
    {
        private readonly ServerDbContext _context;
        private readonly ILogger<IngestDataHandler> _logger;
        private readonly IAlarmPublisher _alarmPublisher;

        public IngestDataHandler(ServerDbContext context, ILogger<IngestDataHandler> logger, IAlarmPublisher publisher)
        {
            _context = context;
            _logger = logger;
            _alarmPublisher = publisher;
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
                AlarmPriority = dto.AlarmPriority
            };

            if (record.AlarmPriority > 0)
            {
                string message = $"[ ALARM - PRIORITY {record.AlarmPriority}] : Sensor: {record.SensorId} | Temperature: {record.Temperature}";
                LogAlarmInColor(message, record.AlarmPriority);
                await _alarmPublisher.PublishAlarmAsync(message);
            }

            try
            {
                await _context.SensorRecords.AddAsync(record, token);
                return await _context.SaveChangesAsync(token) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"[ Ingestion ] Error occured while saving measurements for sensors {dto.SensorId}");
                return false;
            }
        }

        private void LogAlarmInColor(string message, int priority)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = priority switch
            {
                1 => ConsoleColor.Yellow,
                2 => ConsoleColor.DarkYellow,
                3 => ConsoleColor.Red,
                _ => originalColor
            };

            Console.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }
    }
}
