using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.SharedKernel;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventHandlers
{
    public class AllocatePatientsEventHandler : IDomainEventHandler<AllocatePatientsEvent>
    {
        private readonly IPublishEndpoint _publisher;

        public AllocatePatientsEventHandler(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(AllocatePatientsEvent domainEvent, CancellationToken cancellationToken)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
