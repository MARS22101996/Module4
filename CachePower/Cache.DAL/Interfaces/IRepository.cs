using CachePower.DAL.Entities;

namespace CachePower.DAL.Interfaces
{
    public interface IRepository<T> where T : BaseType 
    {
        T Get(int id);

        void Create(T item);
    }
}