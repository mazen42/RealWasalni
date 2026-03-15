using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Wasalni_DataAccess.Data;
using Wasalni_Models;
using Wasalni_Utility;

namespace Wasalni_DataAccess
{
    public class Dbinitializser : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _db;
        private readonly ILogger<Dbinitializser> _logger;

        public Dbinitializser(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            AppDbContext appDbContext,
            ILogger<Dbinitializser> logger)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _db = appDbContext;
            _logger = logger;
        }

        public RoleManager<IdentityRole> Get_roleManager() => _roleManager;

        public async Task InitializeAsync()
        {
            try
            {
                var pending = await _db.Database.GetPendingMigrationsAsync();
                if (pending != null && pending.Any())
                {
                    _logger.LogInformation("Applying {Count} pending migrations.", pending.Count());
                    await _db.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database migration failed during initialization.");
                return;
            }

            try
            {
                // Ensure roles exist
                foreach (var role in new[] { SD.Role_Driver, SD.Role_Admin, SD.Role_Passenger })
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        var r = await _roleManager.CreateAsync(new IdentityRole(role));
                        if (!r.Succeeded)
                            _logger.LogWarning("Failed to create role {Role}: {Errors}", role, string.Join("; ", r.Errors.Select(e => e.Description)));
                        else
                            _logger.LogInformation("Created role {Role}", role);
                    }
                }

                // Ensure admin user exists
                const string adminEmail = "admin@gmail.com";
                var existingAdmin = await _userManager.FindByEmailAsync(adminEmail);
                if (existingAdmin == null)
                {
                    var adminUser = new ApplicationUser
                    {
                        UserName = "admin",
                        Email = adminEmail,
                        PhoneNumber = "89150303",
                        UserType = UserType.Admin,
                        HomeLocation = new Location { Longitude = 31.2489, Latitude = 30.0507 },
                        Age = 21,
                        EmailConfirmed = true
                    };

                    var password = "123mM$@"; // ensure meets Identity password policy
                    var createResult = await _userManager.CreateAsync(adminUser, password);
                    if (!createResult.Succeeded)
                    {
                        _logger.LogWarning("Admin user creation failed: {Errors}", string.Join("; ", createResult.Errors.Select(e => e.Description)));
                        return;
                    }

                    var created = await _userManager.FindByEmailAsync(adminEmail);
                    if (created == null)
                    {
                        _logger.LogWarning("Admin user not found after creation.");
                        return;
                    }

                    var addToRoleResult = await _userManager.AddToRoleAsync(created, SD.Role_Admin);
                    if (!addToRoleResult.Succeeded)
                        _logger.LogWarning("Adding admin user to role failed: {Errors}", string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
                    else
                        _logger.LogInformation("Admin user created and assigned to {Role}. Email={Email}", SD.Role_Admin, adminEmail);
                }
                else
                {
                    _logger.LogDebug("Admin user already exists: {Email}", adminEmail);
                    // ensure user is in Admin role
                    if (!await _userManager.IsInRoleAsync(existingAdmin, SD.Role_Admin))
                    {
                        var addToRoleResult = await _userManager.AddToRoleAsync(existingAdmin, SD.Role_Admin);
                        if (!addToRoleResult.Succeeded)
                            _logger.LogWarning("Adding existing admin to role failed: {Errors}", string.Join("; ", addToRoleResult.Errors.Select(e => e.Description)));
                        else
                            _logger.LogInformation("Existing admin added to role {Role}", SD.Role_Admin);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Initialization failed.");
            }
        }
    }
}
