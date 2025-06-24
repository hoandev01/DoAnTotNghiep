using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // Required for PasswordHasher
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ChickenF.Data;
using ChickenF.Models;
using Hangfire;
using Hangfire.MemoryStorage;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Home/AccessDenied";
    options.LoginPath = "/login"; // hoặc path login của bạn
});

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage(); // Bộ nhớ RAM, không dùng DB
});

builder.Services.AddHangfireServer();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
// Configure DbContext with SQL Server
builder.Services.AddDbContext<FarmContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// Add Authentication
builder.Services.AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.Cookie.Name = "ChickenF.Auth"; // Tên cookie
        options.LoginPath = "/Auth/Login"; // Đường dẫn đến trang đăng nhập
        options.LogoutPath = "/Auth/Logout"; // Đường dẫn đến trang đăng xuất
        options.AccessDeniedPath = "/Home/AccessDenied"; // Đường dẫn khi truy cập bị từ chối
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Thời gian tồn tại của cookie
        options.SlidingExpiration = true; // Gia hạn thời gian tồn tại của cookie nếu người dùng hoạt động
    });

// Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireClaim(ClaimTypes.Role, "1")); // Use ClaimTypes.Role
});
// grant password hash permission for user
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHangfireDashboard();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Add Session middleware

app.UseAuthentication();
app.UseAuthorization();

app.UseStaticFiles();

// Khởi tạo Admin mặc định
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FarmContext>();
    if (!context.Users.OfType<ChickenF.Models.Admin>().Any())
    {
        var passwordHasher = new PasswordHasher<ChickenF.Models.Admin>();
        var admin = new ChickenF.Models.Admin
        {
            Username = "Admin",
            FullName = "Administrator",
            Email = "admin@farm.com",
            Phone = "0941759876"
        };
        admin.Password = passwordHasher.HashPassword(admin, "123");
        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }

}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ROUTER

// Mặc định cho người dùng thường
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Route riêng cho USER (người dùng thường)
app.MapControllerRoute(
    name: "user",
    pattern: "User/{controller=Account}/{action=Index}/{id?}");

// Route cho Admin
app.MapControllerRoute(
    name: "admin_default",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

// Route đăng nhập riêng cho admin
app.MapControllerRoute(
    name: "admin_login",
    pattern: "Admin/{controller=Auth}/{action=Login}/{id?}");


// Auth Routes
app.MapControllerRoute(
    name: "login",
    pattern: "/login",
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "register",
    pattern: "/register",
    defaults: new { controller = "Auth", action = "Register" });

app.MapControllerRoute(
    name: "shop",
    pattern: "shop",
    defaults: new { controller = "Shop", action = "Index" });

app.MapControllerRoute(
    name: "shop_detail",
    pattern: "shop/{id}",
    defaults: new { controller = "Shop", action = "Detail" });
app.MapControllerRoute(
    name: "cart",
    pattern: "cart",
    defaults: new { controller = "Cart", action = "Index" });

app.MapControllerRoute(
    name: "checkout",
    pattern: "checkout",
    defaults: new { controller = "Checkout", action = "Index" });

app.MapControllerRoute(
    name: "checkout_success",
    pattern: "success",
    defaults: new { controller = "Checkout", action = "Success" });


app.Run();
