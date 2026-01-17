using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class DriverTripRequestRepository : Repository<DriverTripRequest>, IDriverTripRequest
    {
        public AppDbContext _db { get; }
        public DriverTripRequestRepository(AppDbContext Db) : base(Db) 
        {
            Db = _db;
        }


        public void Update(DriverTripRequest obj)
        {
            _db.DriverTripRequests.Update(obj);
        }
    }
}
