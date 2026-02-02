using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class Seat
    {
        public int Id { get; set; }
        public SeatChar SeatChar { get; set; }
        public SeatStatus SeatStatus { get; set; }
        public Passenger? Passenger { get; set; }
        public int? PassengerId { get; set; }
        public BusTrip BusTrip { get; set; }
        public int BusTripId { get;set; }
    }
}
