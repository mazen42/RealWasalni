using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class BusTrip
    {
        [Key]
        public int Id { get; set; }
        public int? DriverProfileId { get; set; }
        public DriverProfile? DriverProfile { get; set; }
        public List<Passenger> Passengers { get; set; } = new List<Passenger>();
        public double? Salary { get; set; }
        public RoutePlan RoutePlan { get; set; }
        public string FromGovernerate { get; set; }
        public TimeOnly ArrivalTime { get; set; }
        public string ToGovernerate { get; set; }
        public DateOnly? StartDate {  get; set; }
        public DateOnly? endDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public TripType TripType { get; set; }
        public double? Price { get; set; }
        public TripStatus TripStatus { get; set; }
        public bool NotificationSent { get; set; } = false;
        public VehicleType VehicleType { get; set; }
        public List<Seat> Seats { get; set; }
        public List<Invitation> Invitations { get; set; }

    }
}
