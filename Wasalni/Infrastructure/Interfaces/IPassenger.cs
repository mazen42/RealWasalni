using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IPassenger : IRepository<Passenger>
    {
        void Update (Passenger obj);
    }
}
