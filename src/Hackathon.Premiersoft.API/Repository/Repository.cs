using Hackathon.Premiersoft.API.Data;
using Hackathon.Premiersoft.API.Models.Abstractions;

namespace Hackathon.Premiersoft.API.Repository
{
    internal class Repository<TEntity, Tid> : IRepository<TEntity, Tid> where TEntity : Entity<Tid>
    {
        private readonly IPremiersoftHackathonDbContext _context;

        public Repository(IPremiersoftHackathonDbContext context) => _context = context;

        public void Add(TEntity entity)
        {
            _context.Set<TEntity>().Add(entity);
            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public bool Exists(Tid id)
        {
            return _context.Set<TEntity>().Any(s => s.Id!.Equals(id));
        }

        public IList<TEntity> GetAll()
        {
            return [.. _context.Set<TEntity>()];
        }

        public IList<TEntity> GetAll(int skip, int take)
        {
            return [.. _context.Set<TEntity>().Skip(skip).Take(take)];
        }

        public TEntity? GetById(Tid id)
        {
            return _context.Set<TEntity>().FirstOrDefault(s => s.Id!.Equals(id));
        }

        public long GetCount()
        {
            return _context.Set<TEntity>().LongCount();
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Update(entity);
            _context.SaveChanges();
        }
    }
}
