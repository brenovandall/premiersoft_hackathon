using Hackathon.Premiersoft.API.SharedKernel;

namespace Hackathon.Premiersoft.API.Messaging.Events
{
    public sealed class ImportFileEvent : IDomainEvent
    {
        public string PreSignedUrl { get; set; } = default!;
        public long ImportId { get; set; }
    }
}
