using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class PassengerRepository : Repository<Passenger>, IPassenger
    {
        private AppDbContext _db { get; set; }
        public PassengerRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(Passenger obj)
        {
            _db.Passengers.Update(obj);
        }
    }
}