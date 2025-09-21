using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.SharedKernel;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventHandlers
{
    public class AllocateDoctorsEventHandler : IDomainEventHandler<AllocateDoctorsEvent>
    {
        private readonly IPublishEndpoint  _publisher;

        public AllocateDoctorsEventHandler(IPublishEndpoint publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(AllocateDoctorsEvent domainEvent, CancellationToken cancellationToken)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
