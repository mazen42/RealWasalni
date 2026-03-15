using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface ITicket : IRepository<Ticket>
    {
        public void Update(Ticket ticket);
    }
}
