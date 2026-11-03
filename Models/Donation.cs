using System.ComponentModel.DataAnnotations;

namespace DisasterRelief.Models
{
	public class Donation
	{
		public int Id { get; set; }

		[Required]
		public string DonorName { get; set; }

		[Required]
		[Range(typeof(decimal), "1", "10000000", ErrorMessage = "Please enter an amount of at least 1.")]
		public decimal Amount { get; set; }

		[Required]
		public string DonationType { get; set; } // e.g., Financial, Food, Medical

		// ZAR, USD, GBP or EUR
		[Required]
		public string Currency { get; set; } = "ZAR";

		// "Once-off" or "Monthly" (recurring)
		[Required]
		public string Frequency { get; set; } = "Once-off";

		// Donor chose not to show their name
		public bool IsAnonymous { get; set; }

		// Set when a logged-in donor gives, so they can download their tax certificate later
		public string? DonorEmail { get; set; }

		public DateTime DateDonated { get; set; } = DateTime.Now;

		public static readonly string[] Currencies = { "ZAR", "USD", "GBP", "EUR" };
		public static readonly string[] Frequencies = { "Once-off", "Monthly" };
	}
}