using MediatR;
using Server.Features.Commands;

namespace Server.Features.Handlers
{
    public class IngestDataHandler : IRequestHandler<IngestDataCommand, bool>
    {
        public Task<bool> Handle(IngestDataCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
