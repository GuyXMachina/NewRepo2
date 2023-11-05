using UFSFacilityManagement.Data;
using UFSFacilityManagement.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Builder;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();
builder.Services.AddSession();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
})
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
var config = new FireSharp.Config.FirebaseConfig
{
    AuthSecret = "GO5AfHDQzwiJiNE6bsAoyB3HG8Aj9ViTTJIOKQNG",
    BasePath = "https://facilitymanagement-302af-default-rtdb.firebaseio.com/"
};
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile("facilitymanagement-302af-firebase-adminsdk-vyx60-174d6ae9ca.json"),
});

builder.Services.AddSingleton(new FireSharp.FirebaseClient(config));


var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
StripeConfiguration.ApiKey = Configuration.GetSection("Stripe:SecretKey").Get<string>();
var firebaseApiKey = Configuration["Firebase:ApiKey"];
app.UseAuthentication();
app.UseAuthorization();
app.UseSession(); // Add this line, make sure it's before UseEndpoints()
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapDefaultControllerRoute();
});

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
