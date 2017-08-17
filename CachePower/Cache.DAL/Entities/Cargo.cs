using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CachePower.DAL.Entities;

namespace Caghing.Dal.Entities
{
    [Table("Cargo")]
    public partial class Cargo: BaseType
    {
        [Required]
        [StringLength(50)]
        public string CargoName { get; set; }

        public double CargoWeight { get; set; }

        public double Volume { get; set; }

        public int SenderId { get; set; }

        public int RecipientId { get; set; }

        public int RouteId { get; set; }

        public decimal PriceOfCargo { get; set; }
    }
}
