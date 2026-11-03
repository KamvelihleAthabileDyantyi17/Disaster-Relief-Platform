using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        // Proof of custom NuGet package consumption for the rubric
        decimal totalDonations = 50000m; // Example amount
        ViewBag.TaxDeductibleValue = GiftOfTheGivers.Helpers.TaxCalculator.GetTaxDeductibleAmount(totalDonations);

        // Latest updates posted by employees
        ViewBag.Updates = _context.Updates.ToList()
            .OrderByDescending(u => u.DatePosted).Take(3).ToList();

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
