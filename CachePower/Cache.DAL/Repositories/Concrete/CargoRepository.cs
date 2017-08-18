using System.Linq;
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

        public virtual Cargo GetById(int id)
        {
            var cargo = _context.Cargo.FirstOrDefault(x => x.Id == id);

            return cargo;
        }

        public virtual void Create(Cargo cargo)
        {
            _context.Cargo.Add(cargo);

            _context.SaveChanges();
        }
    }
}