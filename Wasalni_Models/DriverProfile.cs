using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalni_Models
{
    public class DriverProfile
    {
        [Key]
        public int Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        [Required]
        public string LicenseImageFaceURL { get; set; }
        [Required]
        public string LicenseImageBackURL { get; set; }
        [Required]
        public string IdCardFaceURL { get; set; }
        [Required]
        public string IdCardBackURL { get; set; }
        public Bus Bus { get; set; }
        public DateTime SubscriptionExpiryDate { get; set; }
        public bool IsSubscriptionActive => SubscriptionExpiryDate >= DateTime.UtcNow;
        public DriverApprovalStatus ApprovalStatus { get; set; } = DriverApprovalStatus.Pending;
        public List<DriverTripRequest> TripRequests { get; set; }
        public List<BusTrip> busTrips { get; set; }
    }
}
