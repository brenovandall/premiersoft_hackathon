using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public class PremiersoftHackathonDbContext : DbContext, IPremiersoftHackathonDbContext
    {
        public PremiersoftHackathonDbContext(DbContextOptions<PremiersoftHackathonDbContext> options) : base(options) { }
        protected PremiersoftHackathonDbContext() { }

        // tabelas aqui

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // alguma config aqui

            base.OnModelCreating(modelBuilder);
        }
    }
}
