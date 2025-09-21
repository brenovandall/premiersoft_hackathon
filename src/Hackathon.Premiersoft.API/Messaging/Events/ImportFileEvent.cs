using Hackathon.Premiersoft.API.SharedKernel;

namespace Hackathon.Premiersoft.API.Messaging.Events
{
    public sealed record ImportFileEvent(string PreSignedUrl, int FileFormat, long ImportId) : IDomainEvent;
}
