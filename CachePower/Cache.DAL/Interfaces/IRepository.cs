using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface IRepository
    {
        Cargo GetById(int id);

        void Create(Cargo item);
    }
}