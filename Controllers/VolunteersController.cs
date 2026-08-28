using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers
{
	public class VolunteersController : Controller
	{
		private readonly ApplicationDbContext _context;

		public VolunteersController(ApplicationDbContext context)
		{
			_context = context;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(Volunteer volunteer)
		{
			if (ModelState.IsValid)
			{
				_context.Add(volunteer);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Success));
			}
			return View(volunteer);
		}

		public IActionResult Success()
		{
			return View();
		}
	}
}