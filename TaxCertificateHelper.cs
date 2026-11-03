namespace DisasterRelief.Helpers
{
    public static class TaxCertificateHelper
    {
        public static decimal CalculateTaxDeductibleAmount(decimal amount)
        {
            if (amount <= 0) return 0.00m;
            return amount * 0.18m; // 18% tax deduction rule
        }

        public static string FormatCertificateNumber(string donorName, int donationId)
        {
            if (string.IsNullOrEmpty(donorName)) return $"CERT-{donationId}";
            string namePart = donorName.Length >= 4 ? donorName.Substring(0, 4).ToUpper() : donorName.ToUpper();
            return $"CERT-{namePart}-{donationId}";
        }
    }
}