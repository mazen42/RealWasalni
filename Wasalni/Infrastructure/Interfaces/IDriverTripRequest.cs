using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IDriverTripRequest : IRepository<DriverTripRequest>
    {
        void Update (DriverTripRequest request);
    }
}
