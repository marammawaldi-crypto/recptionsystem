using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReceptionSystem.Data;
<<<<<<< HEAD
=======
using ReceptionSystem.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("ApplicationDbContext") ?? throw new InvalidOperationException("Connection string 'ApplicationDbContext' not found.");
>>>>>>> main

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// MVC
// =====================================================

builder.Services.AddControllersWithViews();


// =====================================================
// Database
// =====================================================

var connectionString =
    builder.Configuration.GetConnectionString("ApplicationDbContext")
    ?? throw new InvalidOperationException(
        "Connection string 'ApplicationDbContext' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));


// =====================================================
// Identity
// =====================================================

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;

    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan =
        TimeSpan.FromMinutes(5);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// =====================================================
// Authorization
// =====================================================

builder.Services.AddAuthorization();
<<<<<<< HEAD


// =====================================================
// Identity Cookie
// =====================================================

builder.Services.ConfigureApplicationCookie(options =>
{
    // المستخدم غير المسجل يتم تحويله إلى Login
    options.LoginPath = "/Account/Login";

    // المستخدم المسجل ولكن ليس لديه الصلاحية
    options.AccessDeniedPath = "/Account/AccessDenied";
});


// =====================================================
// Build Application
// =====================================================

=======
builder.Services.AddScoped<JobApplicationNumberGenerator>();
// Add services to the container.
>>>>>>> main
var app = builder.Build();


// =====================================================
// Seed Roles & Default Users
// =====================================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();


    // -------------------------------------------------
    // Roles
    // -------------------------------------------------

    string[] roles =
    {
        "ADMIN",
        "USER",
        "RECEPTIONIST"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // -------------------------------------------------
    // Default Admin
    // -------------------------------------------------

    var testEmail = "admin1@dama.com";

    var existingUser =
        await userManager.FindByEmailAsync(testEmail);

    if (existingUser == null)
    {
        var testUser = new IdentityUser
        {
            UserName = testEmail,
            Email = testEmail,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(
                testUser,
                "Admin@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                testUser,
                "ADMIN");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
                existingUser,
                "ADMIN"))
        {
            await userManager.AddToRoleAsync(
                existingUser,
                "ADMIN");
        }
    }


    // -------------------------------------------------
    // Default Receptionist
    // -------------------------------------------------

    var receptionEmail = "reception@dama.com";

    var existingReception =
        await userManager.FindByEmailAsync(receptionEmail);

    if (existingReception == null)
    {
        var receptionUser = new IdentityUser
        {
            UserName = receptionEmail,
            Email = receptionEmail,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(
                receptionUser,
                "Reception@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                receptionUser,
                "RECEPTIONIST");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
                existingReception,
                "RECEPTIONIST"))
        {
            await userManager.AddToRoleAsync(
                existingReception,
                "RECEPTIONIST");
        }
    }


    // -------------------------------------------------
    // Default User
    // -------------------------------------------------

    var normalUserEmail = "user@dama.com";

    var existingNormalUser =
        await userManager.FindByEmailAsync(normalUserEmail);

    if (existingNormalUser == null)
    {
        var normalUser = new IdentityUser
        {
            UserName = normalUserEmail,
            Email = normalUserEmail,
            EmailConfirmed = true
        };

        var result =
            await userManager.CreateAsync(
                normalUser,
                "User@123");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                normalUser,
                "USER");
        }
    }
    else
    {
        if (!await userManager.IsInRoleAsync(
                existingNormalUser,
                "USER"))
        {
            await userManager.AddToRoleAsync(
                existingNormalUser,
                "USER");
        }
    }
}


// =====================================================
// HTTP Request Pipeline
// =====================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

<<<<<<< HEAD

// =====================================================
// Authentication & Authorization
// =====================================================

app.UseAuthentication();

app.UseAuthorization();
=======
>>>>>>> main


// =====================================================
// Static Files
// =====================================================

app.MapStaticAssets();


// =====================================================
// Default Route
// =====================================================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");


// =====================================================
// Run
// =====================================================

app.Run();