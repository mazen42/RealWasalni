using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models.DTOs
{
    public class DriverRegisterDTO
    {
        public IFormFile LicenseImageFace { get; set; }
        public IFormFile IdCardFace { get; set; }
        public IFormFile LicenseImageBack { get; set; }
        public IFormFile IdCardBack { get; set; }
        public VehicleType VehicleType { get; set; }
        public string PlateNumber { get; set; }
        public int monthsOfWork { get; set; }
    }
}
