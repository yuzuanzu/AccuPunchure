using AccuPunchure.Business.Services;
using AccuPunchure.Data;
using AccuPunchure.Data.Extensions;
using AccuPunchure.Data.Seeders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ACCUPUNCHURE_DB");

// ── Services ───────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();
builder.Services.AddDataServices(connectionString);   // registers DbContext + all repositories

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PunchServices>();
builder.Services.AddScoped<EmployeeService>();

// ── Authentication ─────────────────────────────────────────────────────────────
// Cookie auth: unauthenticated requests are redirected to /Account/Login.
// SlidingExpiration resets the 8-hour window on each request so active users
// stay logged in without interruption.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath        = "/Account/Login";
        options.AccessDeniedPath = "/Account/Login";
        options.ExpireTimeSpan   = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

// ── Migrate + seed ────────────────────────────────────────────────────────────
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();        // apply any pending migrations
        DevSeeder.ClearPunches(db);   // wipe test punches on every restart
        DevSeeder.Seed(db);           // no-op if already seeded
        Console.WriteLine("[Startup] DB migrated and seeded.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Startup] Seeder failed: {ex.Message}");
        throw;
    }
}

// ── Middleware pipeline ────────────────────────────────────────────────────────
// Order matters: Routing → Authentication → Authorization
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();   // reads the cookie and populates HttpContext.User
app.UseAuthorization();    // checks [Authorize] attributes

// /{slug}/login  →  Account/Login
app.MapControllerRoute(
    name: "orgLogin",
    pattern: "{slug}/login",
    defaults: new { controller = "Account", action = "Login" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
