using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class TicketRepository : Repository<Ticket>, ITicket
    {
        private AppDbContext _db;
        public TicketRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(Ticket ticket)
        {
            _db.Tickets.Update(ticket);
        }
    }
}
