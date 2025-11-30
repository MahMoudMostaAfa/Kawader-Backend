
using System.Security.Claims;
using Kawadar.Infrastructure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Kawadar.Infrastructure.Data;


public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    AppDbContext context, UserManager<AppUser> userManager,
    RoleManager<IdentityRole> roleManager)
{
  private readonly ILogger<ApplicationDbContextInitialiser> _logger = logger;
  private readonly AppDbContext _context = context;
  private readonly UserManager<AppUser> _userManager = userManager;
  private readonly RoleManager<IdentityRole> _roleManager = roleManager;

  public async Task InitialiseAsync()
  {
    try
    {
      await _context.Database.EnsureCreatedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while initialising the database.");
      throw;
    }
  }

  public async Task SeedAsync()
  {
    try
    {
      await TrySeedAsync();
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "An error occurred while seeding the database.");
      throw;
    }
  }

  public async Task TrySeedAsync()
  {
    // seeds role 
    _logger.LogInformation("Seeding default roles");

    if (!await _roleManager.RoleExistsAsync(DefaultRoles.User))
      await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.User));

    if (!await _roleManager.RoleExistsAsync(DefaultRoles.Admin))
      await _roleManager.CreateAsync(new IdentityRole(DefaultRoles.Admin));

    // seeds default admin user
    _logger.LogInformation("Seeding default admin user");

    if (await _userManager.FindByEmailAsync("admin@kawadar.com") == null)
    {
      var admin = new AppUser
      {
        UserName = "masteradmin",
        Email = "admin@kawadar.com",
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(admin, "Admin@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(admin, DefaultRoles.Admin);
        foreach (var permission in Permissions.GetAllPermissions())
        {
          await _userManager.AddClaimAsync(admin, new Claim("Permission", permission));
        }
      }
    }

    //  seed default user
    _logger.LogInformation("Seeding default user");

    if (await _userManager.FindByEmailAsync("user@kawadar.com") == null)
    {
      var user = new AppUser
      {
        UserName = "defaultuser",
        Email = "user@kawadar.com",
        EmailConfirmed = true,
      };

      var result = await _userManager.CreateAsync(user, "User@123");
      if (result.Succeeded)
      {
        await _userManager.AddToRoleAsync(user, DefaultRoles.User);
      }
    }
  }
}


public static class InitialiserExtensions
{
  public static async Task InitialiseDatabaseAsync(this WebApplication app)
  {
    using var scope = app.Services.CreateScope();

    var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

    await initialiser.InitialiseAsync();

    await initialiser.SeedAsync();
  }
}