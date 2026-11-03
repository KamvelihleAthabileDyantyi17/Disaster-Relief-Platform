// A.4 Selenium UI Tests - DisasterRelief (Gift of the Givers)
//
// HOW TO RUN
// 1. Start the web app first: in Visual Studio pick the "https" profile, press Ctrl+F5.
//    It should open https://localhost:7039 (from Properties/launchSettings.json).
// 2. Test Explorer > right-click "SeleniumUiTests" > Run.
//    Chrome opens and drives the site by itself. Don't touch it while it runs.
//
// The tests create their own users (unique emails), so you don't need to seed anything.
// They are tagged Category=UI so the Azure pipeline can skip them (no website runs there).

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace DisasterRelief.Tests;

[Trait("Category", "UI")]
public class SeleniumUiTests : IDisposable
{
    private static readonly string BaseUrl =
        (Environment.GetEnvironmentVariable("UI_BASE_URL") ?? "https://localhost:7039").TrimEnd('/');

    private const string TestPassword = "Test@12345"; // meets default Identity password rules

    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;

    public SeleniumUiTests()
    {
        var options = new ChromeOptions();
        options.AddArgument("--ignore-certificate-errors"); // local dev HTTPS certificate
        options.AddArgument("--window-size=1366,900");
        if (Environment.GetEnvironmentVariable("UI_HEADLESS") == "1")
            options.AddArgument("--headless=new");

        // Selenium 4.6+ downloads the matching ChromeDriver automatically.
        _driver = new ChromeDriver(options);
        _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(15));
        _wait.IgnoreExceptionTypes(typeof(NoSuchElementException), typeof(StaleElementReferenceException));
    }

    // ===================== Scenario 1: Employee login =====================

    [Fact]
    public void EmployeeLogin_ValidCredentials_ShowsEmployeeDashboard()
    {
        var email = CreateEmployeeAccount();

        Login(email, TestPassword);
        Go("/Dashboard");

        _wait.Until(d => d.PageSource.Contains("Operations Center"));
        Assert.Contains("Employee Administrator", _driver.PageSource);
        Assert.Contains("Total Platform Contributions", _driver.PageSource);
        Assert.Contains(email, _driver.PageSource);
    }

    [Fact]
    public void EmployeeLogin_WrongPassword_IsRejected()
    {
        var email = CreateEmployeeAccount();

        Login(email, "WrongPassword!1");

        var error = _wait.Until(d => d.FindElement(By.CssSelector(".validation-summary-errors, .text-danger li")));
        Assert.Contains("Invalid login attempt", error.Text);
        Assert.Contains("/Identity/Account/Login", _driver.Url);
    }

    [Fact]
    public void Dashboard_WhenNotLoggedIn_RedirectsToLogin()
    {
        Go("/Dashboard");

        _wait.Until(d => d.Url.Contains("/Identity/Account/Login"));
        Assert.Contains("ReturnUrl", _driver.Url, StringComparison.OrdinalIgnoreCase);
    }

    // ===================== Scenario 2: Anonymous donation =====================

    [Fact]
    public void AnonymousDonation_Submitted_AppearsOnEmployeeDashboard()
    {
        // A visitor who is NOT logged in makes a donation
        var donorLabel = $"Anonymous {DateTime.Now:HHmmssfff}";
        Go("/");
        var form = _wait.Until(d => d.FindElement(By.CssSelector("form[action='/Donations/Create']")));
        form.FindElement(By.Name("DonorName")).SendKeys(donorLabel);
        form.FindElement(By.Name("Amount")).SendKeys("250");
        new SelectElement(form.FindElement(By.Name("DonationType"))).SelectByValue("Financial");
        Click(form.FindElement(By.CssSelector("button[type='submit']")));

        // App redirects back to the home page after saving
        _wait.Until(d => d.FindElements(By.CssSelector("form[action='/Donations/Create']")).Count > 0);

        // An employee logs in and must see the donation in the donor table
        var email = CreateEmployeeAccount();
        Login(email, TestPassword);
        Go("/Dashboard");

        _wait.Until(d => d.PageSource.Contains("Recent Donor Activity"));
        Assert.Contains(donorLabel, _driver.PageSource);
    }

    // ===================== Scenario 3 (bonus): Volunteer registration =====================

    [Fact]
    public void VolunteerRegistration_ValidDetails_ShowsThankYouPage()
    {
        Go("/");
        var form = _wait.Until(d => d.FindElement(By.CssSelector("form[action='/Volunteers/Register']")));
        form.FindElement(By.Name("FirstName")).SendKeys("Thandi");
        form.FindElement(By.Name("LastName")).SendKeys("Selenium");
        form.FindElement(By.Name("EmailAddress")).SendKeys($"volunteer{Guid.NewGuid():N}@test.com");
        form.FindElement(By.Name("PhoneNumber")).SendKeys("0821234567");
        form.FindElement(By.Name("SpecializedSkills")).SendKeys("First Aid");
        Click(form.FindElement(By.CssSelector("button[type='submit']")));

        _wait.Until(d => d.Url.Contains("/Volunteers/Success"));
        Assert.Contains("Thank You for Volunteering", _driver.PageSource);
    }

    // ===================== Helpers =====================

    /// Registers a new user, promotes it to Employee (via /Dashboard/PromoteMe),
    /// then signs out so the test can do a real login. Returns the email.
    private string CreateEmployeeAccount()
    {
        var email = $"employee{Guid.NewGuid():N}@test.com";

        Go("/Identity/Account/Register");
        _wait.Until(d => d.FindElement(By.Id("Input_Email"))).SendKeys(email);
        _driver.FindElement(By.Id("Input_Password")).SendKeys(TestPassword);
        _driver.FindElement(By.Id("Input_ConfirmPassword")).SendKeys(TestPassword);
        Click(_driver.FindElement(By.Id("registerSubmit")));
        _wait.Until(d => !d.Url.Contains("/Identity/Account/Register"));

        Go("/Dashboard/PromoteMe");
        _wait.Until(d => d.PageSource.Contains("You are now an Employee"));

        SignOut();
        return email;
    }

    private void Login(string email, string password)
    {
        Go("/Identity/Account/Login");
        _wait.Until(d => d.FindElement(By.Id("Input_Email"))).SendKeys(email);
        _driver.FindElement(By.Id("Input_Password")).SendKeys(password);
        Click(_driver.FindElement(By.Id("login-submit")));
    }

    private void SignOut()
    {
        Go("/");
        _driver.Manage().Cookies.DeleteAllCookies();
    }

    private void Go(string path) => _driver.Navigate().GoToUrl(BaseUrl + path);

    // Scroll into view first so sticky headers / layout don't block the click
    private void Click(IWebElement element)
    {
        ((IJavaScriptExecutor)_driver).ExecuteScript("arguments[0].scrollIntoView({block:'center'});", element);
        element.Click();
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
