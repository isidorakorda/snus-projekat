using MediatR;
using SensorRegistry.DTOs;

namespace SensorRegistry.Features.Commands
{
    public record AddSensorCommand(SensorRegistrationDTO Data) : IRequest<bool>;
  
}
