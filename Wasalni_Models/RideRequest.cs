using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class RideRequest
    {
        public int Id { get; set; }
        public string FromGovernorate { get; set; }
        public string ToGovernorate { get; set; }
        public TripType TripType { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public VehicleType VehicleType { get; set; }
        public bool IsDone { get; set; } = false;
        public List<Passenger> passengers { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
