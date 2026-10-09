using Microsoft.EntityFrameworkCore; 
using MoviesAdmin.Data; // Using directive for the MoviesAdmin.Data namespace

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MoviesAdminContext>(options =>  // Configure the DbContext to use SQL Server with the connection string from appsettings.json
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("MoviesAdminContext")));

var app = builder.Build();  // Build the application

// Configure the HTTP request pipeline.
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