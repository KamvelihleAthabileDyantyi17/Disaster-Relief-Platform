using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Models;
using System.Linq;
using System.Threading.Tasks;

namespace DisasterRelief.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Inject the database and security managers
        public DashboardController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            // 1. Fetch all donations, newest first
            var allDonations = _context.Donations.ToList()
                .OrderByDescending(d => d.DateDonated).ToList();

            // Older rows (before currency/frequency existed) count as once-off ZAR
            foreach (var d in allDonations)
            {
                if (string.IsNullOrEmpty(d.Currency)) d.Currency = "ZAR";
                if (string.IsNullOrEmpty(d.Frequency)) d.Frequency = "Once-off";
            }

            // 2. Totals: rand total plus a total per currency
            ViewBag.TotalSum = allDonations.Where(d => d.Currency == "ZAR").Sum(d => d.Amount);
            ViewBag.TotalsByCurrency = allDonations
                .GroupBy(d => d.Currency)
                .ToDictionary(g => g.Key, g => g.Sum(d => d.Amount));

            // 3. The logged-in donor's own donations (for tax certificates)
            var me = User?.Identity?.Name;
            ViewBag.MyDonations = allDonations.Where(d => me != null && d.DonorEmail == me).ToList();

            // 4. Send the data to the view
            return View(allDonations);
        }

        // HIDDEN ROUTE: Run this once to make yourself an Employee!
        public async Task<IActionResult> PromoteMe()
        {
            // Create the Employee role in the database if it doesn't exist yet
            if (!await _roleManager.RoleExistsAsync("Employee"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            // Upgrade the currently logged-in user to Employee
            var user = await _userManager.GetUserAsync(User);
            await _userManager.AddToRoleAsync(user, "Employee");

            return Content("Success! You are now an Employee. Please go back, click Logout, and log back in for the changes to take effect.");
        }
    }
}