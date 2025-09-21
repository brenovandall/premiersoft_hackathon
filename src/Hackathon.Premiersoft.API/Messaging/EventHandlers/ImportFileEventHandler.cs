using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.SharedKernel;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventHandlers
{
    public class ImportFileEventHandler : IDomainEventHandler<ImportFileEvent>
    {
        private readonly IPublishEndpoint _publisher;

        public ImportFileEventHandler(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(ImportFileEvent domainEvent, CancellationToken cancellationToken)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
