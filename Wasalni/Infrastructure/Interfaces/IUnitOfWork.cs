using Wasalni.Infrastructure.Repositories;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        public AppDbContext _db {  get; set; }
        public IBus bus {  get; set; }
        public IBusTrip busTrip {  get; set; }
        public IPassenger passenger { get; set; }
        public ITripPoint tripPoint { get; set; }
        public IRoutePlan routePlan { get; set; }
        public IDriverProfile driverProfile { get; set; }
        public IApplicationUser applicationUser { get; set; }
        public IDriverTripRequest driverTripRequest { get; set; }
        public INotificationRepository Notifications { get; set; }
        public ISeat seats { get; set; }
        void Save();

    }
}
