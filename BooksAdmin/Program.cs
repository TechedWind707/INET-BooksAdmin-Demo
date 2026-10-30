using BooksAdmin.Services;
using Microsoft.EntityFrameworkCore;
//using System.Net.Http;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BooksAdminContext") ?? throw new InvalidOperationException("Connection string 'BooksAdminContext' not found.");

builder.Services.AddDbContext<BooksAdminContext>(options => options.UseSqlServer(connectionString));
//Register file upload service
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
