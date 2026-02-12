using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class putThePassengerinTheApproproiateSeatDTO
    {
        public string message {  get; set; }
        public List<Seat> seats { get; set; } = new List<Seat>();
    }
}
