using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class BusRepository : Repository<Bus>, IBus
    {
        private AppDbContext _db { get; set; }
        public BusRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(Bus bus)
        {
            _db.Buses.Update(bus);
        }
    }
}
