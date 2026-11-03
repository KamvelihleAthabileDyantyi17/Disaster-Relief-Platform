// A.1 Unit tests: tax calculation and certificate formatting
using DisasterRelief.Helpers;

namespace DisasterRelief.Tests;

public class TaxCertificateHelperTests
{
    // ---------- Total / tax calculation ----------

    [Fact]
    public void CalculateTaxDeductibleAmount_PositiveAmount_Returns18Percent()
    {
        var result = TaxCertificateHelper.CalculateTaxDeductibleAmount(1000m);

        Assert.Equal(180.00m, result);
    }

    [Theory]
    [InlineData(250, 45)]
    [InlineData(50000, 9000)]
    [InlineData(100, 18)]
    public void CalculateTaxDeductibleAmount_VariousAmounts_AreCorrect(int amount, int expected)
    {
        Assert.Equal((decimal)expected, TaxCertificateHelper.CalculateTaxDeductibleAmount(amount));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-500)]
    public void CalculateTaxDeductibleAmount_ZeroOrNegative_ReturnsZero(int amount)
    {
        Assert.Equal(0.00m, TaxCertificateHelper.CalculateTaxDeductibleAmount(amount));
    }

    // ---------- Certificate formatting ----------

    [Fact]
    public void FormatCertificateNumber_LongName_UsesFirstFourLettersUppercase()
    {
        var cert = TaxCertificateHelper.FormatCertificateNumber("Thandiwe", 42);

        Assert.Equal("CERT-THAN-42", cert);
    }

    [Fact]
    public void FormatCertificateNumber_ShortName_UsesWholeNameUppercase()
    {
        var cert = TaxCertificateHelper.FormatCertificateNumber("Bo", 7);

        Assert.Equal("CERT-BO-7", cert);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FormatCertificateNumber_NoName_ReturnsIdOnly(string? donorName)
    {
        var cert = TaxCertificateHelper.FormatCertificateNumber(donorName!, 15);

        Assert.Equal("CERT-15", cert);
    }
}
