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
        public int DriverProfileId { get; set; }
        public DriverProfile DriverProfile { get; set; }
        public List<Passenger> Passengers { get; set; } = new List<Passenger>();
        public double? Salary { get; set; }
        public RoutePlan RoutePlan { get; set; }
        public string FromGovernerate { get; set; }
        public string ToGovernerate { get; set; }
        public DateOnly? StartDate {  get; set; }
        public DateOnly endDate { get; set; } = DateOnly.FromDateTime(DateTime.Now.AddMonths(1));
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsOptimized { get; set; } = false; 
        public bool IsConfirmed { get; set; } = false; 
        public double? Price { get; set; }
        public bool NotificationSent { get; set; } = false;
        public bool IsCompleted() => this.Passengers.Count == 14;

    }
}
