using MediatR;

namespace SensorRegistry.Features.Commands
{
    public record SetSensorStatusCommand(List<Guid> Ids, bool IsActive) : IRequest<bool>;
}
