using Microsoft.EntityFrameworkCore;

namespace Hackathon.Premiersoft.API.Data
{
    public interface IPremiersoftHackathonDbContext
    {
        // tabelas aqui

        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }
}
