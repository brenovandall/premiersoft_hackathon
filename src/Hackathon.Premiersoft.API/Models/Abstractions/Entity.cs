using Hackathon.Premiersoft.API.SharedKernel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hackathon.Premiersoft.API.Models.Abstractions
{
    public abstract class Entity<T> : IEntity<T>
    {
        private readonly List<IDomainEvent> _domainEvents = [];
        public List<IDomainEvent> DomainEvents => [.. _domainEvents];

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; } = default!;

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        public void Raise(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
