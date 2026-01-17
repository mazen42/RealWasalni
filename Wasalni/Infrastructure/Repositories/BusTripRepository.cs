using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class BusTripRepository : Repository<BusTrip>, IBusTrip
    {
        private AppDbContext _db { get; set; }
        public BusTripRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(BusTrip obj)
        {
            _db.BusTrips.Update(obj);
        }
    }
}