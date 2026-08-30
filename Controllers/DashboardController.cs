using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DisasterRelief.Controllers
{
	[Authorize] // Requires login to access anything in this controller
	public class DashboardController : Controller
	{
		public IActionResult Index()
		{
			// Employees and Donors see different data on the view
			return View();
		}

		[Authorize(Roles = "Employee")] // Only employees can hit this endpoint
		[HttpPost]
		public IActionResult PostUpdate(string message)
		{
			// Logic to save disaster updates goes here
			return RedirectToAction("Index");
		}
	}
}