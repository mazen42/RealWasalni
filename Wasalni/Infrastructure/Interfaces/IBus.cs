using Newtonsoft.Json.Bson;
using Wasalni_Models;

namespace Wasalni.Infrastructure.Interfaces
{
    public interface IBus : IRepository<Bus>
    {
        void Update(Bus bus);
    }
}
