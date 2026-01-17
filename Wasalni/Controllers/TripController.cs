using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_Models;
using Wasalni_Models.DTOs;
using Wasalni_Utility;

namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Passenger")]
    public class TripController : ControllerBase
    {
        private IUnitOfWork _un {  get; set; }
        private readonly HttpClient _httpClient;

        public TripController(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _un = unitOfWork;
            this._httpClient = httpClient;
        }
        [HttpPost("UserTripRequest")]
        public async Task<IActionResult> UserTripRequestAsync(TripRequestDTO obj)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            var responseFrom = await BuiltInMethods.GetCityFromNominatimAsync(obj.FromLocation.Latitude, obj.FromLocation.Longitude,_httpClient);
            var responseTo = await BuiltInMethods.GetCityFromNominatimAsync(obj.ToLocation.Latitude, obj.ToLocation.Longitude,_httpClient);
            var PassengerRepeatingCheck = await _un.passenger.Get(x => x.FromLocation.Longitude == obj.FromLocation.Longitude && x.FromLocation.Latitude == obj.FromLocation.Latitude && x.ToLocation.Longitude == obj.ToLocation.Longitude && x.ToLocation.Latitude == obj.ToLocation.Latitude && x.ApplicationUserId == User.GetUserId() && x.ArrivalTime == obj.arrivalTime);
            if (PassengerRepeatingCheck is not null)
                return BadRequest("Trip Already Requested");
            Passenger passenger = new Passenger
            {
                ArrivalTime = obj.arrivalTime,
                TripType = obj.TripType,
                FromLocation = new Location { Latitude = obj.FromLocation.Latitude, Longitude = obj.FromLocation.Longitude },
                ToLocation = new Location { Latitude = obj.ToLocation.Latitude, Longitude = obj.ToLocation.Longitude },
                ApplicationUserId = User.GetUserId(),
            };
            _un.passenger.Add(passenger);
            _un.Save();
            var RideRequestsCheck = _un.RideRequest.GetAll(r => r.FromGovernorate == responseFrom && r.ToGovernorate == responseTo && r.ArrivalTime == obj.arrivalTime && r.VehicleType == obj.VehicleType && obj.TripType == r.TripType && r.passengers.Count() < 14 , includeProperties: "passengers").OrderBy(o => o.CreatedAt).FirstOrDefault();
            if(RideRequestsCheck != null)
            {
                passenger.RideRequestId = RideRequestsCheck.Id;
                _un.Save();
                return Ok(new {message = "Trip Booked Successfully u Will Get Notification When its done",code = 204});
            }
            var newRide = new RideRequest
            {
                ArrivalTime = obj.arrivalTime,
                FromGovernorate = responseFrom!,
                ToGovernorate = responseTo!,
                CreatedAt = DateTime.UtcNow,
                TripType = obj.TripType,
                VehicleType = obj.VehicleType,
            };
            _un.RideRequest.Add(newRide);
            _un.Save();
            passenger.RideRequestId = newRide.Id;
            _un.Save();
            return Ok();
        }
    }
}
