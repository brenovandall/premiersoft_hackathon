using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public class PremiersoftHackathonDbContext : DbContext, IPremiersoftHackathonDbContext
    {
        public PremiersoftHackathonDbContext(DbContextOptions<PremiersoftHackathonDbContext> options) : base(options) { }
        protected PremiersoftHackathonDbContext() { }

        DbSet<Estados> Estados { get; set; }
        DbSet<Municipios> Cidades { get; set; }
        DbSet<Medicos> Medicos { get; set; }
        DbSet<Hospitais> Hospitais { get ; set; }
        DbSet<Pacientes> Pacientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
