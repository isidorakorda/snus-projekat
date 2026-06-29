using MediatR;
using SensorRegistry.Data;
using SensorRegistry.Features.Queries;
using SensorRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace SensorRegistry.Features.Handlers
{
    public class GetFaultySensorsHandler : IRequestHandler<GetFaultySensorsQuery, List<Guid>>
    {
        private readonly SensorDbContext _context;

        public GetFaultySensorsHandler(SensorDbContext context)
        {
            _context = context;
        }

        public async Task<List<Guid>> Handle(GetFaultySensorsQuery request, CancellationToken cancellationToken)
        {
            List<Guid> sensorIds = request.Sensors
                .Where(s => s.IsActive)
                .Select(s => s.Id)
                .ToList();

            return await _context.SensorRecords
                .Where(record => sensorIds.Contains(record.SensorId))
                .GroupBy(record => record.SensorId)
                .Where(r => r.Max(record => record.Timestamp) < request.Timeout)
                .Select(r => r.Key)
                .ToListAsync(cancellationToken);
        }
    }
}
