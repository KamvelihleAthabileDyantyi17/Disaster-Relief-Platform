using System.ComponentModel.DataAnnotations;

namespace DisasterRelief.Models
{
	public class Donation
	{
		public int Id { get; set; }

		[Required]
		public string DonorName { get; set; }

		[Required]
		public decimal Amount { get; set; }

		[Required]
		public string DonationType { get; set; } // e.g., Financial, Food, Medical

		public DateTime DateDonated { get; set; } = DateTime.Now;
	}
}