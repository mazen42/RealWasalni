using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {

        public AppDbContext _db { get; set; }
        public IBus bus { get; set; }
        public IBusTrip busTrip { get; set; }
        public IPassenger passenger { get; set; }
        public ITripPoint tripPoint { get; set; }
        public IRoutePlan routePlan { get; set; }
        public IDriverProfile driverProfile { get; set; }
        public IApplicationUser applicationUser { get; set; }
        public IDriverTripRequest driverTripRequest { get; set; }
        public INotificationRepository Notifications { get; set; }
        public ITicket tickets { get; set; }
        public ISeat seats {  get; set; }
        public IInvitation invitation { get; set; }

        public UnitOfWork(AppDbContext appDbContext)
        {
            _db = appDbContext;
            bus = new BusRepository(_db);
            busTrip = new BusTripRepository(_db);
            passenger = new PassengerRepository(_db);
            tripPoint = new TripPointRepository(_db);
            routePlan = new RoutePlanRepository(_db);
            driverProfile = new DriverProfileRepository(_db);
            applicationUser = new ApplicationUserRepository(_db);
            driverTripRequest = new DriverTripRequestRepository(_db);
            Notifications = new NotificationRepository(_db);
            seats = new SeatRepository(_db);
            tickets = new TicketRepository(_db);
            invitation = new InvitaitonRepository(_db);
        }


        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
