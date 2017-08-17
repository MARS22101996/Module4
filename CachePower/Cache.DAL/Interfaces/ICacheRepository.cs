using System.Collections.Generic;
using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface ICacheRepository
    {
        void Set(Cargo entity);

        CachedEntity Get(int id);

        IEnumerable<CachedEntity> PopAllCreated();

        void SetAsCreated(Cargo entity);

        IEnumerable<CachedEntity> GetAll();
    }
}