using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Caghing.Dal.Context;
using Caghing.Dal.Entities;
using Caghing.Dal.Interfaces;

namespace Caghing.Dal.Repositories
{
    public class CargoRepository : ICargoRepository
    {
        private readonly ShipmentContext _context;

        public CargoRepository(ShipmentContext context)
        {
            _context = context;
        }

        public Cargo GetById(int id)
        {
            return _context.Cargo.FirstOrDefault(x => x.Id == id);
        }

        public void Create(Cargo item)
        {
            _context.Cargo.Add(item);
            _context.SaveChanges();
        }
    }
}
