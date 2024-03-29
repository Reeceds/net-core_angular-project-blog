using System.Text.Json.Serialization;
using Admin;
using Microsoft.AspNetCore.Identity;
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

// ! Add for Identity user and role configuration e.g. password rules, remove 'options => {}' for no configuraiton
builder.Services.AddIdentity<AppUser, IdentityRole>(
    options =>
    {
        options.Password.RequiredUniqueChars = 0;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 5;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireLowercase = false;
        options.User.RequireUniqueEmail = true;
    }
).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();

// ! Add for Identity redirect of path unauthorized routes
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
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
