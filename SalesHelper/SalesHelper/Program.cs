using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using SalesHelper.Data;
using SalesHelper.Models;
using SalesHelper.Services;
using SalesHelper.Services.EmailService;
using SalesHelper.Services.QuestPDF;
using SalesHelper.Services.UserServices;
using Microsoft.AspNetCore.Identity;
using SalesHelper.Services.EncryptionServices;

QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// database related services
builder.Services.AddScoped<VendorService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AddressService>();
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<CabinetQuotationService>();
builder.Services.AddScoped<CountertopQuotationService>();
builder.Services.AddScoped<SalesService>();
// additional services
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<GeneratePDFService>();
builder.Services.AddScoped<ProfileCheckService>();
builder.Services.AddScoped<EncryptionDecryptionService>();

// For uploading larg files to the server
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});
// For uploading larg files to the IIS Server
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = int.MaxValue;
});

//Adding Time Span for Confirmation Link
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromHours(24);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
