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
        public DbSet<DoctorsHospitals> DoctorsHospitals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DoctorsHospitals>()
                .HasOne(dh => dh.Doctor)
                .WithMany(m => m.DoctorsHospitals)
                .HasForeignKey(dh => dh.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DoctorsHospitals>()
                .HasOne(dh => dh.Hospital)
                .WithMany(h => h.DoctorsHospitals)
                .HasForeignKey(dh => dh.HospitalId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PatientsHospitals>()
                .HasOne(ph => ph.Patient)
                .WithMany(p => p.PatientsHospitals)
                .HasForeignKey(ph => ph.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PatientsHospitals>()
                .HasOne(ph => ph.Hospital)
                .WithMany(h => h.PatientsHospitals)
                .HasForeignKey(ph => ph.HospitalId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }

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
