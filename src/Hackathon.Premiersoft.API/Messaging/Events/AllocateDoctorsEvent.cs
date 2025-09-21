using Hackathon.Premiersoft.API.SharedKernel;

namespace Hackathon.Premiersoft.API.Messaging.Events
{
    public sealed record AllocateDoctorsEvent(Guid Id) : IDomainEvent;
}
