using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface ISeat : IRepository<Seat>
    {
        void Update(Seat seat);
    }
}
