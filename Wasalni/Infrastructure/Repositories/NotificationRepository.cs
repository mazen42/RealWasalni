using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Repositories
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private AppDbContext AppDbContext { get; set; }
        public NotificationRepository(AppDbContext appDbContext):base(appDbContext) { 
            AppDbContext = appDbContext;
        }

        public void Update(Notification notification)
        {
            AppDbContext.Update(notification);
        }
    }
}
