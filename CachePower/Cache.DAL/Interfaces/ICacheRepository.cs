using System.Collections.Generic;
using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface ICacheRepository<TEntity> where TEntity : BaseType, new()
    {
        void Set(TEntity entity);

        CachedEntity<TEntity> Get(int id);

        IEnumerable<CachedEntity<TEntity>> PopAllCreated();

        void SetAsCreated(TEntity entity);

        IEnumerable<CachedEntity<TEntity>> GetAll();
    }
}