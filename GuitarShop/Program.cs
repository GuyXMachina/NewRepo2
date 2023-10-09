using GuitarShop.Data;
using GuitarShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddSession();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();
//Database Option 1: SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.Configure<StripeSettings>(Configuration.GetSection("Stripe"));


builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.AppendTrailingSlash = true;
});

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
StripeConfiguration.ApiKey = Configuration.GetSection("Stripe:SecretKey").Get<string>();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Add this line, make sure it's before UseEndpoints()

// route for sorting and paging category
app.MapControllerRoute(
        name: "sortingpagingcategory",
        pattern: "{controller}/{action}/{id}/orderby{sortBy}/page{productPage}");

// route for paging all products
app.MapControllerRoute(
name: "allpaging",
pattern: "{controller}/{action}/{id=all}/page{productPage}");

// route for sorting
app.MapControllerRoute(
    name: "sortingcategory",
    pattern: "{controller}/{action}/{id}/orderby{sortBy}");

// least specific route - 0 required segments 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}/{slug?}");

SeedData.EnsurePopulated(app);
SeedData.CreateRolesAndUsers(app);
app.Run();
