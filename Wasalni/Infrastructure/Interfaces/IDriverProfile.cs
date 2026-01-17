using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IDriverProfile : IRepository<DriverProfile>
    {
        void Update (DriverProfile obj);
    }
}
