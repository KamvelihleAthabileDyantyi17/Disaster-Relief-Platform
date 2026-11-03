// A.2 Integration tests: form submission > controller > EF Core > database > dashboard
// Uses the real controllers and the real ApplicationDbContext, with an in-memory database
// so the tests run anywhere (Visual Studio and the Azure pipeline) without SQL Server.
using DisasterRelief.Controllers;
using DisasterRelief.Data;
using DisasterRelief.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DisasterRelief.Tests;

public class DonationsIntegrationTests
{
    private static ApplicationDbContext NewDatabase() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"DisasterReliefTest-{Guid.NewGuid()}") // fresh DB per test
            .Options);

    [Fact]
    public async Task DonationFlow_SubmitForm_SavesDonationToDatabase()
    {
        using var db = NewDatabase();
        var controller = new DonationsController(db);
        var form = new Donation { DonorName = "Anonymous", Amount = 500m, DonationType = "Financial" };

        var result = await controller.Create(form);

        // Controller sends the donor back to the home page
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("Home", redirect.ControllerName);

        // ...and the donation is really in the database
        var saved = await db.Donations.SingleAsync();
        Assert.Equal("Anonymous", saved.DonorName);
        Assert.Equal(500m, saved.Amount);
        Assert.True(saved.Id > 0);
    }

    [Fact]
    public async Task DonationFlow_InvalidForm_DoesNotSaveToDatabase()
    {
        using var db = NewDatabase();
        var controller = new DonationsController(db);
        controller.ModelState.AddModelError("DonationType", "The DonationType field is required.");

        await controller.Create(new Donation { DonorName = "Sipho", Amount = 100m, DonationType = "" });

        Assert.Equal(0, await db.Donations.CountAsync());
    }

    [Fact]
    public async Task DonationFlow_SeveralDonations_DashboardShowsCorrectTotal()
    {
        using var db = NewDatabase();
        var donations = new DonationsController(db);

        await donations.Create(new Donation { DonorName = "Anonymous", Amount = 250m, DonationType = "Financial" });
        await donations.Create(new Donation { DonorName = "Sipho Dlamini", Amount = 1000m, DonationType = "Medical" });
        await donations.Create(new Donation { DonorName = "Acme Foods", Amount = 750.50m, DonationType = "Food" });

        // Dashboard.Index only uses the database, so the identity managers aren't needed here
        var dashboard = new DashboardController(db, null!, null!);
        var view = Assert.IsType<ViewResult>(dashboard.Index());

        var model = Assert.IsAssignableFrom<IEnumerable<Donation>>(view.Model);
        Assert.Equal(3, model.Count());
        Assert.Equal(2000.50m, (decimal)view.ViewData["TotalSum"]!);
    }

    [Fact]
    public async Task DonationFlow_AnonymousRecurringForeignCurrency_SavedCorrectly()
    {
        using var db = NewDatabase();
        var controller = new DonationsController(db);
        var form = new Donation
        {
            DonorName = "Should be hidden", IsAnonymous = true,
            Amount = 50m, Currency = "USD", Frequency = "Monthly", DonationType = "Financial"
        };

        await controller.Create(form);

        var saved = await db.Donations.SingleAsync();
        Assert.Equal("Anonymous", saved.DonorName);
        Assert.Equal("USD", saved.Currency);
        Assert.Equal("Monthly", saved.Frequency);
    }

    [Fact]
    public async Task DonationFlow_UnsupportedCurrency_IsRejected()
    {
        using var db = NewDatabase();
        var controller = new DonationsController(db);

        await controller.Create(new Donation { DonorName = "Test", Amount = 50m, Currency = "BTC", DonationType = "Financial" });

        Assert.Equal(0, await db.Donations.CountAsync());
    }

    [Fact]
    public async Task VolunteerFlow_SubmitForm_SavesVolunteerAndShowsSuccess()
    {
        using var db = NewDatabase();
        var controller = new VolunteersController(db);
        var form = new Volunteer
        {
            FirstName = "Thandi", LastName = "Mokoena", EmailAddress = "thandi@test.com",
            PhoneNumber = "0821234567", SpecializedSkills = "First Aid"
        };

        var result = await controller.Register(form);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Success", redirect.ActionName);
        Assert.Equal("thandi@test.com", (await db.Volunteers.SingleAsync()).EmailAddress);
    }
}
