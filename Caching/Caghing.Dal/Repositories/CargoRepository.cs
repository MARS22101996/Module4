using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Caghing.Dal.Context;
using Caghing.Dal.Entities;
using Caghing.Dal.Interfaces;

namespace Caghing.Dal.Repositories
{
    public class CargoRepository : ICargoRepository, IDisposable
    {
        private ShipmentContext _context;

        public CargoRepository(ShipmentContext context)
        {
            _context = context;
        }

        public IEnumerable<Cargo> GetAll()
        {
            return _context.Cargo.ToList();
        }

        public Cargo GetById(int id)
        {
            return _context.Cargo.FirstOrDefault(x => x.Id == id);
        }

        public void Create(Cargo item)
        {
            _context.Cargo.Add(item);
        }

        public void Delete(int id)
        {
            var item = _context.Cargo.Find(id);
            if (item == null) return;
            _context.Cargo.Remove(item);
        }

        public void Update(Cargo item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_context == null) return;
            _context.Dispose();
            _context = null;
        }
    }
}
