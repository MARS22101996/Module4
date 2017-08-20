using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Cache.DAL.Context;
using Cache.DAL.Entities;
using Cache.DAL.Repositories.Interfaces;

namespace Cache.DAL.Repositories.Concrete
{
    public class CargoRepository : IRepository
    {
        private readonly ShipmentContext _context;

        public CargoRepository(ShipmentContext context)
        {
            _context = context;
        }

        public Task<Cargo> GetByIdAsync(int id)
        {
            var cargo = _context.Cargo.FirstOrDefaultAsync(x => x.Id == id);

            return cargo;
        }

        public Task CreateAsync(Cargo cargo)
        {
            _context.Cargo.Add(cargo);

             return _context.SaveChangesAsync();
        }
    }
}