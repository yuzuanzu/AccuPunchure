using AccuPunchure.Data;
using Microsoft.EntityFrameworkCore;
using AccuPunchure.Data.Extensions;

var builder = WebApplication.CreateBuilder(args);

//Connection
var connectionString = Environment.GetEnvironmentVariable("ACCUPUNCHURE_DB");

// Services
builder.Services.AddControllersWithViews();
builder.Services.AddDataServices(connectionString);

var app = builder.Build();

// Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Esrror");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();