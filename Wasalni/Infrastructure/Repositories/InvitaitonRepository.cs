using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class InvitaitonRepository : Repository<Invitation>, IInvitation
    {
        private AppDbContext _db;
        public InvitaitonRepository(AppDbContext appDbContext) : base(appDbContext)
        {
            _db = appDbContext;
        }
        public void Update(Invitation invitation)
        {
            _db.Update(invitation);
        }
    }
}
