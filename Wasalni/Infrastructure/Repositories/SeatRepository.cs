using System.Linq.Expressions;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class SeatRepository : Repository<Seat>, ISeat
    {
        private AppDbContext _db { get; set; }
        public SeatRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(Seat obj)
        {
            _db.Seats.Update(obj);
        }
    }
}
