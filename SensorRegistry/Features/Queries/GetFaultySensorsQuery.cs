using MediatR;
using SensorRegistry.DTOs;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Queries
{
    public record GetFaultySensorsQuery(List<SensorDTO> Sensors, DateTime Timeout) : IRequest<List<Guid>>;

}
