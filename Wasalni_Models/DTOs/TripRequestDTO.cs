using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class TripRequestDTO
    {
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly arrivalTime { get; set; }
        public LocationDTO FromLocation { get; set; }
        public LocationDTO ToLocation { get; set; }
        public VehicleType VehicleType { get; set; }
        public TripType TripType { get; set; }
    }
}
