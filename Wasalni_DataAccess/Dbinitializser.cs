using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalni_DataAccess.Data;
using Wasalni_Models;
using Wasalni_Utility;
namespace Wasalni_DataAccess
{
    public class Dbinitializser : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly AppDbContext _db;

        public Dbinitializser(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, AppDbContext appDbContext)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            this._db = appDbContext;
        }

        public RoleManager<IdentityRole> Get_roleManager()
        {
            return _roleManager;
        }

        public async Task InitializeAsync()
        {
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex) { 
            
            }
            if (!await _roleManager.RoleExistsAsync(SD.Role_Driver))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Driver)).ConfigureAwait(true);
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).ConfigureAwait(true);
                await _roleManager.CreateAsync(new IdentityRole(SD.Role_Passenger)).ConfigureAwait(true);
                await _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@gmail.com",
                    PhoneNumber = "89150303",
                    UserType = UserType.Admin,
                    HomeLocation = new Location {Longitude = 31.2489 ,Latitude = 30.0507 },
                    Age = 21,
                }, "123mM$@");

                ApplicationUser? user = await _userManager.FindByEmailAsync("admin@gmail.com");
                await _userManager.AddToRoleAsync(user!, SD.Role_Admin);
            };

        }
    }
}
