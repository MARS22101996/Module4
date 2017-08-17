using System.Collections.Generic;
using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface ICacheCargoRepository
    {
        void Set(Cargo entity);

        CachedCargo Get(int id);

        IEnumerable<CachedCargo> PopAllCreated();

        void SetAsCreated(Cargo entity);

        IEnumerable<CachedCargo> GetAll();
    }
}