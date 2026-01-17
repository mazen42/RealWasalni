using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Bson;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IApplicationUser : IRepository<ApplicationUser>
    {
        void Update(ApplicationUser applicationUser);
    }
}
