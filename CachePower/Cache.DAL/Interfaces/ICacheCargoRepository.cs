using System.Collections.Generic;
using Cache.DAL.Entities;

namespace Cache.DAL.Interfaces
{
    public interface ICacheCargoRepository
    {
        void Configure(Cargo entity);

        CachedCargo GetById(int id);

        IEnumerable<CachedCargo> PopAllCreated();

        void SetAsCreated(Cargo entity);

        IEnumerable<CachedCargo> GetAll();
    }
}