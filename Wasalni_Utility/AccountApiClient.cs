using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Utility
{
    public class AccountApiClient
    {
        public HttpClient Client { get; }

        public AccountApiClient(HttpClient client)
        {
            Client = client;
        }
    }
}
