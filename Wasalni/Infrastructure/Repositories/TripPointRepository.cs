using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class TripPointRepository : Repository<TripPoint>, ITripPoint
    {
        private AppDbContext _db { get; set; }
        public TripPointRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(TripPoint obj)
        {
            _db.TripPoints.Update(obj);
        }
    }
}