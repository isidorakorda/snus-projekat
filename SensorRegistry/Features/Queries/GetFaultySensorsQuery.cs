using MediatR;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Queries
{
    public record GetFaultySensorsQuery(DateTime Timeout) : IRequest<List<Sensor>>;

}
