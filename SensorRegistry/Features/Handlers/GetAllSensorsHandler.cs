using MediatR;
using SensorRegistry.Data;
using SensorRegistry.DTOs;
using SensorRegistry.Features.Queries;
using SensorRegistry.Models;
using Microsoft.EntityFrameworkCore;

namespace SensorRegistry.Features.Handlers
{
    public class GetAllSensorsHandler : IRequestHandler<GetAllSensorsQuery, List<SensorDTO>>
    {
        private readonly SensorDbContext _context;

        public GetAllSensorsHandler(SensorDbContext context)
        {
            _context = context;
        }

        public async Task<List<SensorDTO>> Handle(GetAllSensorsQuery request, CancellationToken token)
        {
            IQueryable<Sensor> query = _context.Sensors.AsQueryable();

            if (request.IsActive.HasValue) query = query.Where(s => s.IsActive == request.IsActive.Value);

            List<Sensor> sensors = query.ToList();

            return await query.Select(model => new SensorDTO
            {
                Id = model.Id,
                Quality = model.Quality,
                IsActive = model.IsActive,
                IsMalicious = model.IsMalicious,
                DateTimeOfRegistration = model.DateTimeOfRegistration,
                LastSeen = model.LastSeen,
                DeactivationTime = model.DeactivationTime,
                PublicKey = model.PublicKey
            }).ToListAsync(token);

        }
    }
}
