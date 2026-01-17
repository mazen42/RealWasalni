using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IBusTrip : IRepository<BusTrip>
    {
        void Update (BusTrip obj);
    }
}
