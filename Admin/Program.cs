using System.Text.Json.Serialization;
using Admin;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// ! Add .AddJsonOptions(... when including related table data (.Include()) in API calls to avoid serialization error
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ! Add this connection string before 'var app = builder.Build();' is run
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlite("Data source=net-core_angular-project.db");
});

// ! Add CORS
builder.Services.AddCors();

var app = builder.Build();

// ! Add CORS for the url of the front end (i.e the local server that Angular is running on)
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowCredentials() // SignalR related
.WithOrigins("http://localhost:4200"));

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
