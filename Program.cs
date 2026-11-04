using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Data;
using MoviesAdmin.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Configure Auth0 authentication.
builder.Services
    .AddAuth0WebAppAuthentication(options =>
    {
        options.Domain =
            builder.Configuration["Auth0:Domain"]!;

        options.ClientId =
            builder.Configuration["Auth0:ClientId"]!;

        options.ClientSecret =
            builder.Configuration["Auth0:ClientSecret"]!;
    });

// Configure Auth0 role claims.
builder.Services.Configure<OpenIdConnectOptions>(
    Auth0Constants.AuthenticationScheme,
    options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                NameClaimType = "name",
                RoleClaimType = "http://localhost:5071/roles"
            };
    });

// Add authorization services.
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy =
        new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
});

// Configure SQL Server database.
builder.Services.AddDbContext<MoviesAdminContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MoviesAdminContext")));

// Register Azure Blob Storage service.
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Authenticate the user before authorization is checked.
app.UseAuthentication();

app.UseAuthorization();

// Static assets must remain available
// to anonymous users so the public pages
// can load CSS, JavaScript, and images.
app.MapStaticAssets()
    .Add(endpointBuilder =>
       endpointBuilder.Metadata.Add(new AllowAnonymousAttribute()));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();