using Hackathon.Premiersoft.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public interface IPremiersoftHackathonDbContext
    {
        DbSet<Estados> Estados { get; }
        DbSet<Municipios> Cidades { get; }
        DbSet<Medicos> Medicos { get; }
        DbSet<Hospitais> Hospitais { get; }
        DbSet<Pacientes> Pacientes { get; }
        DbSet<Import> Imports { get; }
        DbSet<LineError> LineErrors { get; }
        DbSet<Lookup> Lookups { get; }

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
