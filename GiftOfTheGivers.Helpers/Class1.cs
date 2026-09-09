namespace GiftOfTheGivers.Helpers
{
    public static class TaxCalculator
    {
        // Calculates an 18% tax-deductible certificate value for donations
        public static decimal GetTaxDeductibleAmount(decimal donationAmount)
        {
            return donationAmount * 0.18m;
        }
    }
}