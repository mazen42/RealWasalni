using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class Invitation
    {
        public int Id { get; set; }
        public TimeOnly ExpiresAt { get; set; }
        public ApplicationUser Sender { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Receiver { get; set; }
        public string ReceiverId { get; set; }
        public InvitationStatus Status { get; set; }
        public BusTrip? BusTrip { get; set; }
        public SeatChar seatChar { get; set; }
        public int? BusTripId { get; set; }
    }
}
