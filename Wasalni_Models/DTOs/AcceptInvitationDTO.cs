using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class AcceptInvitationDTO
    {
        public int TripId { get; set; }
        public SeatChar seatChar { get; set; }
        public int InvitationId { get; set; }
    }
}
