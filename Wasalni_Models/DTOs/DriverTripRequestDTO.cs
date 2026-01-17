using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class DriverTripRequestDTO
    {
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly StartTime { get; set; }
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly EndTime { get; set; }
        [Required]
        public LocationDTO FromLocation { get; set; }
        [Required]
        public LocationDTO ToLocation { get; set; }
        

    }
}
