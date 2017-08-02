using System.Collections.Generic;
using Caghing.Dal.Entities;

namespace Caghing.Dal.Interfaces
{
    public interface ICargoRepository
    {
        void Create(Cargo item);

        Cargo GetById(int id);
    }
}