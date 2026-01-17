using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class TripPoint
    {
        [Key]
        public int Id { get; set; }
        public int RoutePlanId { get; set; }
        public RoutePlan RoutePlan { get; set; }
        [Required]
        public PointType PointType { get; set; }
        [Required]
        public double Latitude { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public TimeOnly Time { get; set; }

    }
}
