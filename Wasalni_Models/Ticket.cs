using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public string Ticketguid { get; set; }
        public Passenger Passenger { get; set; }
        public int PassengerId { get; set; }
        public bool QRstatus { get; set; } = false;
    }
}
