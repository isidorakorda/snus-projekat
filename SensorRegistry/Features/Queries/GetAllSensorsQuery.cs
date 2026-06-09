using MediatR;
using SensorRegistry.DTOs;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Queries
{
    public record GetAllSensorsQuery(bool? IsActive) : IRequest<List<SensorDTO>>;

}
