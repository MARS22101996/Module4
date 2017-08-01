using System.Collections.Generic;
using Caghing.Dal.Entities;

namespace Caghing.Dal.Interfaces
{
    public interface ICargoRepository
    {
        void Create(Cargo item);
        void Update(Cargo item);
        void Delete(int id);
        IEnumerable<Cargo> GetAll();
        Cargo GetById(int id);
        void Save();
    }
}