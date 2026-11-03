using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers
{
    public class DonationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Handles the "Make a donation" form on the home page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Donation donation)
        {
            // Anonymous donation: ticked the box or left the name empty
            if (donation.IsAnonymous || string.IsNullOrWhiteSpace(donation.DonorName))
            {
                donation.IsAnonymous = true;
                donation.DonorName = "Anonymous";
                ModelState.Remove(nameof(Donation.DonorName));
            }

            if (!Donation.Currencies.Contains(donation.Currency))
                ModelState.AddModelError(nameof(Donation.Currency), "Unsupported currency.");
            if (!Donation.Frequencies.Contains(donation.Frequency))
                ModelState.AddModelError(nameof(Donation.Frequency), "Unsupported frequency.");

            // Link the donation to the logged-in donor so they can download a tax certificate
            if (User?.Identity?.IsAuthenticated == true)
                donation.DonorEmail = User.Identity.Name;

            if (ModelState.IsValid)
            {
                _context.Add(donation);
                await _context.SaveChangesAsync();
                SetMessage(donation.Frequency == "Monthly"
                    ? $"Thank you! Your monthly donation of {donation.Currency} {donation.Amount:0.00} has been set up."
                    : $"Thank you! Your donation of {donation.Currency} {donation.Amount:0.00} has been received.");
            }
            else
            {
                SetMessage("Your donation could not be processed. Please check the amount and try again.");
            }

            return RedirectToAction("Index", "Home");
        }

        private void SetMessage(string message)
        {
            if (TempData != null) TempData["DonationMessage"] = message;
        }
    }
}