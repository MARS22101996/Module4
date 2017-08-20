using System.Threading.Tasks;
using Cache.DAL.Entities;

namespace Cache.DAL.Repositories.Interfaces
{
    public interface IRepository
    {
        Task<Cargo> GetByIdAsync(int id);

        Task CreateAsync(Cargo item);
    }
}