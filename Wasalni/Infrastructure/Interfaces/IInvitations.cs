using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IInvitation : IRepository<Invitation>
    {
        public void Update(Invitation invitation);
    }
}
