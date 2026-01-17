using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class DriverProfileRepository:Repository<DriverProfile>,IDriverProfile
    {
        private AppDbContext _db { get; set; }
        public DriverProfileRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(DriverProfile obj)
        {
            _db.DriverProfiles.Update(obj);
        }
    }
}
