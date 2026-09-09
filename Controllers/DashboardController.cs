using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
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
            // 1. Fetch all donations from the database
            var allDonations = _context.Donations.ToList();

            // 2. Calculate the total sum of all amounts
            ViewBag.TotalSum = _context.Donations.Sum(d => d.Amount);

            // 3. Send the data to the view
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