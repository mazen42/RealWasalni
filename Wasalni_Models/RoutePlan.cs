using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class RoutePlan
    {
        [Key]
        public int Id { get; set; }
        public List<TripPoint> PickUpPoints { get; set; }
        public List<TripPoint> DropOffPoints { get; set; }
        public int BusTripId {  get; set; }
        public BusTrip BusTrip { get; set; }
        [Required]
        public TimeOnly ExpectedArrivalTime {  get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
