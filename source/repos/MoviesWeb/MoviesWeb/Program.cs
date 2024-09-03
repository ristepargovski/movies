using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.Cookies;
using MoviesWeb.Models.System;
using MoviesWeb.Services;

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
		  .AddCookie(options =>
		  {
			  options.Cookie.HttpOnly = true;
			  options.Cookie.SameSite = SameSiteMode.Lax;
            
			  options.LoginPath = "/Login/Login";



              options.ExpireTimeSpan = TimeSpan.FromMinutes(double.Parse("1440"));
			  options.SlidingExpiration = true;
		  });
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromSeconds(1800);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});


builder.Services.AddHttpClient(); 
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<CxSettings>(builder.Configuration.GetSection("CxSettings"));

builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();


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
app.UseSession();
app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
       name: "search",
       pattern: "{controller=Movies}/{action=SearchResults}/{query?}");

app.Run();
