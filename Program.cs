using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DisasterRelief.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure SQL Server Database Connection using Entity Framework Core
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DisasterReliefDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

// 2. Register ASP.NET Core Identity services for User Authentication and Role-Based Access Control (RBAC)
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3. Register MVC Controllers and Razor Pages services for the presentation layer
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 4. Configure the HTTP request pipeline for development vs production environments
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts(); // Enforces HTTP Strict Transport Security in production
}

app.UseHttpsRedirection(); // Redirects all HTTP traffic to HTTPS securely
app.UseStaticFiles();     // Enables serving static files (CSS, JavaScript, images)
app.UseRouting();         // Enables endpoint routing to match incoming URLs

// 5. CRITICAL MIDDLEWARE ORDER: Authentication must execute before Authorization
app.UseAuthentication();
app.UseAuthorization();

// 6. Map default MVC controller routes and Razor Pages endpoints
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

// 7. Expose partial Program class to support WebApplicationFactory integration testing
public partial class Program { }