using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public class PremiersoftHackathonDbContext : DbContext, IPremiersoftHackathonDbContext
    {
        public PremiersoftHackathonDbContext(DbContextOptions<PremiersoftHackathonDbContext> options) : base(options) { }
        protected PremiersoftHackathonDbContext() { }
        public DbSet<Estados> Estados { get; set; }
        public DbSet<Municipios> Cidades { get; set; }
        public DbSet<Medicos> Medicos { get; set; }
        public DbSet<Hospitais> Hospitais { get ; set; }
        public DbSet<Pacientes> Pacientes { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<LineError> LineErrors { get; set; }
        public DbSet<Lookup> Lookups { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
