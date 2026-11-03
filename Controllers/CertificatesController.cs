using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DisasterRelief.Data;
using DisasterRelief.Helpers;
using DisasterRelief.Models;

namespace DisasterRelief.Controllers
{
    // Section 18A-style tax certificate for a donation
    [Authorize]
    public class CertificatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CertificatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // View the certificate on screen (with Print / Save as PDF)
        public async Task<IActionResult> Show(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();
            if (!CanAccess(donation)) return Forbid();

            ViewBag.CertificateNumber = TaxCertificateHelper.FormatCertificateNumber(donation.DonorName, donation.Id);
            ViewBag.Deductible = TaxCertificateHelper.CalculateTaxDeductibleAmount(donation.Amount);
            return View(donation);
        }

        // Download the certificate as a file
        public async Task<IActionResult> Download(int id)
        {
            var donation = await _context.Donations.FindAsync(id);
            if (donation == null) return NotFound();
            if (!CanAccess(donation)) return Forbid();

            var certNo = TaxCertificateHelper.FormatCertificateNumber(donation.DonorName, donation.Id);
            var html = BuildCertificateHtml(donation, certNo,
                TaxCertificateHelper.CalculateTaxDeductibleAmount(donation.Amount));
            return File(Encoding.UTF8.GetBytes(html), "text/html", $"{certNo}.html");
        }

        // Employees can access every certificate; donors only their own
        private bool CanAccess(Donation donation) =>
            User.IsInRole("Employee") ||
            (donation.DonorEmail != null && donation.DonorEmail == User.Identity?.Name);

        private static string BuildCertificateHtml(Donation d, string certNo, decimal deductible)
        {
            string e(string s) => WebUtility.HtmlEncode(s);
            var currency = string.IsNullOrEmpty(d.Currency) ? "ZAR" : d.Currency;
            return $@"<!doctype html><html><head><meta charset=""utf-8""><title>{e(certNo)}</title>
<style>body{{font-family:Georgia,serif;max-width:700px;margin:40px auto;color:#1b3a2b}}
h1{{text-align:center}} table{{width:100%;border-collapse:collapse;margin-top:24px}}
td{{padding:10px;border-bottom:1px solid #ddd}} td:first-child{{font-weight:bold;width:45%}}
.box{{border:3px double #1b3a2b;padding:32px}}</style></head><body><div class=""box"">
<h1>Gift of the Givers Foundation</h1><h2 style=""text-align:center"">Donation Tax Certificate</h2>
<table>
<tr><td>Certificate number</td><td>{e(certNo)}</td></tr>
<tr><td>Donor</td><td>{e(d.DonorName)}</td></tr>
<tr><td>Date of donation</td><td>{d.DateDonated:dd MMMM yyyy}</td></tr>
<tr><td>Donation type</td><td>{e(d.DonationType)}</td></tr>
<tr><td>Amount</td><td>{e(currency)} {d.Amount:0.00}</td></tr>
<tr><td>Tax-deductible value (18%)</td><td>{e(currency)} {deductible:0.00}</td></tr>
</table><p style=""margin-top:32px;font-size:13px"">Issued electronically by the Disaster Relief Platform on {DateTime.Now:dd MMMM yyyy}.</p>
</div></body></html>";
        }
    }
}