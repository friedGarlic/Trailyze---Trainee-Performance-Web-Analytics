using Microsoft.EntityFrameworkCore;
using ML_ASP.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using ML_ASP.DataAccess;
using ML_ASP.DataAccess.Repositories.IRepositories;
using ML_ASP.DataAccess.Repositories;
using Microsoft.AspNetCore.Identity;
using ML_ASP.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

/*builder.Services.AddAuthentication(option =>
{
    option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
}).AddCookie().AddGoogle(GoogleDefaults.AuthenticationScheme, option =>
{
    option.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
    option.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value; 
});*/

//SCOPES
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddDbContext<ApplicationDBContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDBContext>();

builder.Services.AddSingleton<IEmailSender, EmailSender>();


builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

builder.Services.AddSession();

var app = builder.Build();

app.UseAuthentication();

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

app.UseSession();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id = UrlParameter.Optional }");

app.Run();
