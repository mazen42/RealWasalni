using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface ITripPoint : IRepository<TripPoint>
    {
        void Update (TripPoint obj);
    }
}
