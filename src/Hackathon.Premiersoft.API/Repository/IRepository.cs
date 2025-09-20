using Hackathon.Premiersoft.API.Models.Abstractions;
using System.Reflection.Metadata;

namespace Hackathon.Premiersoft.API.Repository
{
    public interface IRepository<TEntity, Tid> where TEntity : Entity<Tid>
    {
        void Add(TEntity entity);
        void Delete(TEntity entity);
        bool Exists(Tid id);
        IList<TEntity> GetAll();
        IList<TEntity> GetAll(int skip, int take);
        TEntity? GetById(Tid id);
        long GetCount();
        void Update(TEntity entity);
    }
}
