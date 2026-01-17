using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class RideRequestRepository : Repository<RideRequest>, IRideRequest
    {
        private AppDbContext _db { get; set; }
        public RideRequestRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(RideRequest RideRequest)
        {
            _db.RideRequests.Update(RideRequest);
        }
    }
}
