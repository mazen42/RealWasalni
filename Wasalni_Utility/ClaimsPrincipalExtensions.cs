using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Wasalni_DataAccess.Data;

namespace Wasalni_Utility
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal @this)
        {
            return @this.FindFirst(ClaimTypes.NameIdentifier).Value;
        }
        public static int GetDriverId(this ClaimsPrincipal @this,AppDbContext _db) {
            var driverId = _db.DriverProfiles.FirstOrDefault(d => d.ApplicationUserId == @this.FindFirst(ClaimTypes.NameIdentifier).Value).Id;
            return driverId;
        }
    }
}
