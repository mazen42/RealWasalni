using Microsoft.AspNetCore.Identity;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUser
    {
        public AppDbContext _db { get; set; }
        public ApplicationUserRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(ApplicationUser applicationUser)
        {
            _db.ApplicationUsers.Update(applicationUser);
        }
    }
}