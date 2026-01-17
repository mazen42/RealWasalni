using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IRoutePlan : IRepository<RoutePlan>
    {
        void Update(RoutePlan obj);
    }
}
