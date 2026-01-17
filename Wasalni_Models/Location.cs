using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class Location
    {
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public override string ToString()
        {
            return $"{this.Latitude},{this.Longitude}";
        }
    }
}
