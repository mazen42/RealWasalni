using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class DriverTripRequest
    {
        public int Id { get; set; }
        public DriverProfile Driver { get; set; }
        public int DriverId { get; set; }
        public string FromGovernerate { get; set; }
        public string ToGovernerate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DriverTripRequestStatus RequestStatus { get; set; } = DriverTripRequestStatus.Requested;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
