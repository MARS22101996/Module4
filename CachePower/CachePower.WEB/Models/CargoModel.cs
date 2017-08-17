using System.ComponentModel.DataAnnotations;

namespace Cache.WEB.Models
{
    public class CargoModel
    {
        public int Id { get; set; }

		[Required]	
		public string CargoName { get; set; }

		[Required]
		public double CargoWeight { get; set; }

		[Required]
		public double Volume { get; set; }

		[Required]
		public int SenderId { get; set; }

		[Required]
		public int RecipientId { get; set; }

		[Required]
		public int RouteId { get; set; }

		[Required]
		public decimal PriceOfCargo { get; set; }
	}
}