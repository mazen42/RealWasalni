using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class RoutePlanRepository : Repository<RoutePlan>, IRoutePlan
    {
        private AppDbContext _db { get; set; }
        public RoutePlanRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(RoutePlan obj)
        {
            _db.RoutePlans.Update(obj);
        }
    }
}