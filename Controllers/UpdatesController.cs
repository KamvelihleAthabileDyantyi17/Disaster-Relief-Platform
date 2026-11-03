using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers
{
    // Only employees can post updates
    [Authorize(Roles = "Employee")]
    public class UpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UpdatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Create()
        {
            return View(new ReliefUpdate());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReliefUpdate update)
        {
            if (!ModelState.IsValid) return View(update);

            update.PostedBy = User?.Identity?.Name;
            update.DatePosted = DateTime.Now;
            _context.Updates.Add(update);
            await _context.SaveChangesAsync();

            if (TempData != null) TempData["DashboardMessage"] = "Your update has been posted to the home page.";
            return RedirectToAction("Index", "Dashboard");
        }
    }
}