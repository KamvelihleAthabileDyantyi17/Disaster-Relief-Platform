using Microsoft.EntityFrameworkCore;
using DisasterRelief.Models;

namespace DisasterRelief.Data
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<Volunteer> Volunteers { get; set; }
		public DbSet<Donation> Donations { get; set; }
	}
}