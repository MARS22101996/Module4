using Cache.DAL.Context;
using Cache.DAL.Entities;
using Cache.DAL.Interfaces;

namespace Cache.DAL.Repositories
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
            var cargo = _context.Cargo.Find(id);

            return cargo;
        }

        public virtual void Create(Cargo cargo)
        {
            _context.Cargo.Add(cargo);

            _context.SaveChanges();
        }
    }
}