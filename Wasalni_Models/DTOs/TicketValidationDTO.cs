using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class TicketValidationDTO
    {
        [Required]
        public string TicketNumber { get; set; }
        [Required]
        public int TripId { get; set; }
    }
}
