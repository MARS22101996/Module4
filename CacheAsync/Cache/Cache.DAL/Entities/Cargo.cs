using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cache.DAL.Entities
{
    [Table("Cargo")]
    public class Cargo
    {
		public int Id { get; set; }

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
