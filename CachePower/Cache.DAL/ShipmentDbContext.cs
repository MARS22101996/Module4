using System.Data.Entity;
using CachePower.DAL.Entities;

namespace CachePower.DAL
{
    [DbConfigurationType(typeof(AppDbConfiguration))]
    public class ShipmentDbContext : DbContext
    {
        public ShipmentDbContext() 
            : base("name=ShipmentDbConnection")
        {
            
        }

        public virtual DbSet<Cargo> Cargoes { get; set; }
    }
}