using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IRideRequest : IRepository<RideRequest>
    {
        void Update (RideRequest obj);
    }
}
