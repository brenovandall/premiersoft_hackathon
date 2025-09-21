using Hackathon.Premiersoft.API.Models;
using Hackathon.Premiersoft.API.Models.Abstractions;
using Hackathon.Premiersoft.API.SharedKernel;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public class PremiersoftHackathonDbContext : DbContext, IPremiersoftHackathonDbContext
    {
        private readonly IDomainEventsDispatcher _domainEventsDispatcher;

        public PremiersoftHackathonDbContext(
            DbContextOptions<PremiersoftHackathonDbContext> options,
            IDomainEventsDispatcher domainEventsDispatcher) : base(options)
        {
            _domainEventsDispatcher = domainEventsDispatcher;
        }

        protected PremiersoftHackathonDbContext(IDomainEventsDispatcher domainEventsDispatcher)
        {
            _domainEventsDispatcher = domainEventsDispatcher;
        }

        public DbSet<Estados> Estados { get; set; }
        public DbSet<Municipios> Cidades { get; set; }
        public DbSet<Medicos> Medicos { get; set; }
        public DbSet<Hospitais> Hospitais { get ; set; }
        public DbSet<Pacientes> Pacientes { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<LineError> LineErrors { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);

            // aceita múltiplos complex types de Entity :)
            await PublishDomainEvents<Guid>();

            return result;
        }

        public async Task PublishDomainEvents<T>()
        {
            var domainEvents = ChangeTracker
            .Entries<Entity<T>>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                List<IDomainEvent> domainEvents = entity.DomainEvents;

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

            await _domainEventsDispatcher.DispatchAsync(domainEvents);
        }
    }
}
