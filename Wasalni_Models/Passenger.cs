using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class Passenger
    {
        public int Id { get; set; }
        [Required]
        public Location FromLocation { get; set; }
        [Required]
        public Location ToLocation { get; set; }
        [Required]
        public TimeOnly ArrivalTime { get; set; }
        public RideRequest RideRequest { get; set; }
        public int? RideRequestId { get; set; }
        public int DaysLeft { get; set; } = 30;
        public bool IsTransfared { get; set; } = false;
        public TripType TripType { get; set; }
        public BusTrip BusTrip { get; set; }
        public int? BusTripId { get; set; }
        public bool IsPaid { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Seat? Seat { get; set; }
    }
}
