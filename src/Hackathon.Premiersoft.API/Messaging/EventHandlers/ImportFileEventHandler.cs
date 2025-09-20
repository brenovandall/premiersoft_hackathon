using Hackathon.Premiersoft.API.Messaging.Events;
using Hackathon.Premiersoft.API.SharedKernel;
using MassTransit;

namespace Hackathon.Premiersoft.API.Messaging.EventHandlers
{
    public class ImportFileEventHandler : IDomainEventHandler<ImportFileEvent>
    {
        private readonly IPublishEndpoint _publisher;

        public ImportFileEventHandler(IPublishEndpoint publisher) => _publisher = publisher;

        public async Task Handle(ImportFileEvent domainEvent, CancellationToken cancellationToken)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Head, domainEvent.PreSignedUrl);
            var response = await client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                await _publisher.Publish(domainEvent, cancellationToken);
            }
            else
            {
                await _publisher.Publish(domainEvent, ctx =>
                {
                    ctx.Delay = TimeSpan.FromSeconds(30);
                }, cancellationToken);
            }
        }
    }
}
