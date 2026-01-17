using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface INotificationRepository : IRepository<Notification>
    {
        void Update(Notification notification);
    }
}
