using System.Data.Entity;
using CachePower.DAL.Entities;

namespace CachePower.DAL.Context
{
    public partial class ShipmentContext : DbContext
    {
        public ShipmentContext()
            : base("name=Module4DbConnection")
        {
        }

        public virtual DbSet<Cargo> Cargo { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Cargo>()
                .Property(e => e.CargoName)
                .IsUnicode(false);

            modelBuilder.Entity<Cargo>()
                .Property(e => e.PriceOfCargo)
                .HasPrecision(10, 2);
        }
    }
}
