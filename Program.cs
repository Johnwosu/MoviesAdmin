using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Data;
using MoviesAdmin.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

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

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();