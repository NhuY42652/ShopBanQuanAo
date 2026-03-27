using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopBanQuanAoOnline.Data;
using ShopBanQuanAoOnline.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddSingleton<IPasswordHasher<Khachhang>, PasswordHasher<Khachhang>>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();

    var useSqlScriptInitializer = app.Configuration.GetValue<bool>("Database:UseSqlScriptInitializer");
    if (useSqlScriptInitializer)
    {
        var scriptRelativePath = app.Configuration.GetValue<string>("Database:InitScriptPath")
            ?? "scripts/db/ShopBanQuanAoOnline_Init.sql";
        var scriptFullPath = Path.Combine(app.Environment.ContentRootPath, scriptRelativePath);

        await DbScriptInitializer.InitializeFromScriptAsync(connectionString, scriptFullPath);
    }
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();