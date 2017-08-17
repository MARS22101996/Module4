using CachePower.DAL.Entities;
using CachePower.DAL.Interfaces;

namespace CachePower.DAL.Repositories
{
    public class CargoRepository : IRepository
    {
        private readonly ShipmentDbContext _context;

        public CargoRepository(ShipmentDbContext context)
        {
            _context = context;
        }

        public virtual Cargo Get(int id)
        {
            var cargo = _context.Cargoes.Find(id);

            return cargo;
        }

        public virtual void Create(Cargo cargo)
        {
            _context.Cargoes.Add(cargo);
            _context.SaveChanges();
        }
    }
}