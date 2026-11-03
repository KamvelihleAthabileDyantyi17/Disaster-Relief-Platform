// A.1 Unit tests: donation validation (the same rules MVC checks when the form is posted)
using System.ComponentModel.DataAnnotations;
using DisasterRelief.Models;

namespace DisasterRelief.Tests;

public class DonationValidationTests
{
    private static List<ValidationResult> Validate(object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        return results;
    }

    [Fact]
    public void Donation_WithAllRequiredFields_IsValid()
    {
        var donation = new Donation { DonorName = "Anonymous", Amount = 250m, DonationType = "Financial" };

        Assert.Empty(Validate(donation));
    }

    [Fact]
    public void Donation_MissingDonorName_IsInvalid()
    {
        var donation = new Donation { DonorName = null!, Amount = 250m, DonationType = "Financial" };

        var errors = Validate(donation);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Donation.DonorName)));
    }

    [Fact]
    public void Donation_MissingDonationType_IsInvalid()
    {
        var donation = new Donation { DonorName = "Anonymous", Amount = 100m, DonationType = null! };

        var errors = Validate(donation);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Donation.DonationType)));
    }

    [Fact]
    public void Donation_NewDonation_DefaultsDateToNow()
    {
        var donation = new Donation { DonorName = "Anonymous", Amount = 100m, DonationType = "Food" };

        Assert.True((DateTime.Now - donation.DateDonated).TotalSeconds < 5);
    }

    [Fact]
    public void Donation_ZeroAmount_IsInvalid()
    {
        var donation = new Donation { DonorName = "Anonymous", Amount = 0m, DonationType = "Financial" };

        var errors = Validate(donation);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Donation.Amount)));
    }

    [Fact]
    public void Volunteer_InvalidEmail_IsInvalid()
    {
        var volunteer = new Volunteer
        {
            FirstName = "Thandi", LastName = "Mokoena", EmailAddress = "not-an-email",
            PhoneNumber = "0821234567", SpecializedSkills = "First Aid"
        };

        var errors = Validate(volunteer);

        Assert.Contains(errors, e => e.MemberNames.Contains(nameof(Volunteer.EmailAddress)));
    }
}
