using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DisasterRelief.Functions
{
    public class TaxCertificateFunction
    {
        private readonly ILogger<TaxCertificateFunction> _logger;

        public TaxCertificateFunction(ILogger<TaxCertificateFunction> logger)
        {
            _logger = logger;
        }

        [Function("GenerateTaxCertificate")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a tax certificate request.");

            string donorName = req.Query["donorName"];
            string amountStr = req.Query["amount"];

            if (string.IsNullOrEmpty(donorName))
            {
                donorName = "Generous Donor";
            }

            if (!decimal.TryParse(amountStr, out decimal amount))
            {
                amount = 1000.00m;
            }

            decimal taxDeductible = amount * 0.18m;
            string certificateNumber = $"GOTG-TAX-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var certificate = new
            {
                CertificateNumber = certificateNumber,
                Donor = donorName,
                TotalDonation = amount,
                TaxDeductibleAmount = taxDeductible,
                IssuedDate = DateTime.UtcNow,
                Message = "Thank you for supporting Gift of the Givers Disaster Relief."
            };

            return new OkObjectResult(certificate);
        }
    }
}