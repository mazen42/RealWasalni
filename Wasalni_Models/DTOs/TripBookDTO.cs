using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class TripBookDTO
    {
        public string? fromGovernerate {  get; set; }
        public string? toGovernerate { get; set; }
        public int CharIndex { get; set; }
        public LocationDTO? FromLocation { get; set; }
        public LocationDTO? ToLocation { get; set; }
        public int? tripId { get; set; }
        [JsonConverter(typeof(TimeOnlyJsonConverter))]
        public TimeOnly arrivalTime { get; set; } = new TimeOnly(DateTime.Now.Hour);
        public VehicleType VehicleType { get; set; } = VehicleType.Bus;
        public TripType TripType { get; set; } = TripType.Single;
    }
}
