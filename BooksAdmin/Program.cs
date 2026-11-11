using Auth0.AspNetCore.Authentication;
//using BooksAdmin;
using BooksAdmin.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
//using System.Net.Http;
var builder = WebApplication.CreateBuilder(args);
Console.WriteLine(builder.Configuration.GetConnectionString("BooksAdminContext"));
var connectionString = builder.Configuration.GetConnectionString("BooksAdminContext") ?? throw new InvalidOperationException("Connection string 'BooksAdminContext' not found.");

builder.Services.AddDbContext<BooksAdminContext>(options => options.UseSqlServer(connectionString));
//Register file upload service
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();
// Register new service
builder.Services.AddScoped<IUserSnapshotService, UserSnapshotService>();

builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddAuthentication();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];

    options.Scope = "openid profile email";
    options.OpenIdConnectEvents = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var principal = context.Principal!;

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.Identity?.Name;

            if (string.IsNullOrEmpty(userId))
                return;

            var syncService = context.HttpContext.RequestServices
                .GetRequiredService<IUserSnapshotService>();

            await syncService.AddUserAsync(userId, email, name);
        }
    };
});
builder.Services.Configure<OpenIdConnectOptions>(Auth0Constants.AuthenticationScheme, options =>
{
    options.Events.OnRemoteFailure = context =>
    {
        context.HandleResponse();

        context.Response.Redirect(
            "/?loginError=cancelled");

        return Task.CompletedTask;
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "name",
        RoleClaimType = "https://localhost:7210/roles" // change to your app's domain
    };
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
