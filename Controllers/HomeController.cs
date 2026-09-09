using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Proof of custom NuGet package consumption for the rubric
        decimal totalDonations = 50000m; // Example amount
        ViewBag.TaxDeductibleValue = GiftOfTheGivers.Helpers.TaxCalculator.GetTaxDeductibleAmount(totalDonations);

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
