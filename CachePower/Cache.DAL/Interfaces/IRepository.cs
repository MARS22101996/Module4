using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface IRepository
    {
        Cargo Get(int id);

        void Create(Cargo item);
    }
}