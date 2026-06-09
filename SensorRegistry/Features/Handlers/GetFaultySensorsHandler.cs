using MediatR;
using SensorRegistry.Data;
using SensorRegistry.Features.Queries;
using SensorRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace SensorRegistry.Features.Handlers
{
    public class GetFaultySensorsHandler : IRequestHandler<GetFaultySensorsQuery, List<Sensor>>
    {
        private readonly SensorDbContext _context;

        public GetFaultySensorsHandler(SensorDbContext context)
        {
            _context = context;
        }

        public async Task<List<Sensor>> Handle(GetFaultySensorsQuery request, CancellationToken cancellationToken)
        {
            return await _context.Sensors
                .Where(s => s.IsActive && s.DateTimeOfRegistration < request.Timeout)
                .ToListAsync(cancellationToken);
        }
    }
}
