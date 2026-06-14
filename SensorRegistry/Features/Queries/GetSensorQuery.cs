using MediatR;
using SensorRegistry.Models;

namespace SensorRegistry.Features.Queries
{
    public record GetSensorQuery(Guid SensorId) : IRequest<string?>
    {
    }
}
