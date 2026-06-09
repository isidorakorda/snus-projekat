using MediatR;
using Server.DTO;

namespace Server.Features.Commands
{
    public record IngestDataCommand(IngestDataDTO Data) : IRequest<bool>;
}
