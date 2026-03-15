using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Threading.Tasks;
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
        private IUnitOfWork _un { get; set; }
        private readonly HttpClient _httpClient;

        public TripController(IUnitOfWork unitOfWork, HttpClient httpClient)
        {
            _un = unitOfWork;
            this._httpClient = httpClient;
        }
        [HttpPost("UserTripRequestSingle")]
        public async Task<IActionResult> UserTripRequestAsync(TripRequestDTO obj)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var responseFrom = await BuiltInMethods.GetCityFromNominatimAsync(obj.FromLocation.Latitude, obj.FromLocation.Longitude, _httpClient);
            var responseTo = await BuiltInMethods.GetCityFromNominatimAsync(obj.ToLocation.Latitude, obj.ToLocation.Longitude, _httpClient);

            var PassengerRepeatingCheck = await _un.passenger.Get(x => x.FromLocation.Longitude == obj.FromLocation.Longitude && x.FromLocation.Latitude == obj.FromLocation.Latitude && x.ToLocation.Longitude == obj.ToLocation.Longitude && x.ToLocation.Latitude == obj.ToLocation.Latitude && x.ApplicationUserId == User.GetUserId() && x.ArrivalTime == obj.arrivalTime);
            if (PassengerRepeatingCheck is not null)
                return BadRequest(new { message = "Trip Already Requested", code = BadRequest().StatusCode });
            if (responseFrom is null || responseTo is null)
                return BadRequest(new { code = BadRequest().StatusCode, message = "Invalid Location" });

            var ExsitsTripsCheck = _un.busTrip.GetAll(
    r => r.FromGovernerate == responseFrom &&
         r.ToGovernerate == responseTo &&
         r.ArrivalTime == obj.arrivalTime &&
         r.VehicleType == obj.VehicleType &&
         r.TripType == obj.TripType &&
         r.Passengers.Count() < 14 &&
         r.TripStatus == TripStatus.Pending,
    includeProperties: "Passengers,Seats"
).OrderBy(o => o.CreatedAt).FirstOrDefault();
            var objToSend = new
            {
                fromGovernerate = responseFrom,
                toGovernerate = responseTo,
                arrivalTime = obj.arrivalTime,
                vehicleType = obj.VehicleType,
                tripType = obj.TripType,
                fromLocaation = obj.FromLocation,
                toLocation = obj.ToLocation,
            };
            if (ExsitsTripsCheck != null)
            {
                var seatsDict = ExsitsTripsCheck.Seats.putThePassengerinTheApproproiateSeatSingle();
                return Ok(new { message = seatsDict, tripId = ExsitsTripsCheck.Id, tripData = objToSend, code = Ok().StatusCode });
            }
            else
            {
                var dict = new Dictionary<string, bool>
            {
                { "A", false },
                { "B", false },
                { "C", false },
                { "D", false },
                { "E", false },
                { "F", false },
                { "G", false },
                { "H", false },
                { "I", false },
                { "L", false },
                { "M", false },
                { "N", false },
                };

                return Ok(new { message = dict, tripId = 0, tripData = objToSend, code = Ok().StatusCode });
            }
        }
        [HttpPost("BookTheTripWithSeatChar")]
        public async Task<IActionResult> BookTheTripWithSeatChar(TripBookDTO obj)
        {
            var ticket = new Ticket
            {
                Ticketguid = Guid.NewGuid().ToString()
            };
            if (obj.tripId == 0)
            {
                var busTrip = new BusTrip
                {
                    ArrivalTime = obj.arrivalTime,
                    FromGovernerate = obj.fromGovernerate!,
                    ToGovernerate = obj.toGovernerate,
                    TripStatus = TripStatus.Pending,
                    CreatedAt = DateTime.Now,
                    VehicleType = obj.VehicleType,
                    TripType = obj.TripType,
                };
                _un.busTrip.Add(busTrip);
                _un.Save();
                var passenger = new Passenger
                {
                    ApplicationUserId = User.GetUserId(),
                    ArrivalTime = obj.arrivalTime,
                    DaysLeft = 30,
                    TripType = obj.TripType,
                    BusTripId = busTrip.Id,
                };
                _un.passenger.Add(passenger);
                _un.Save();
                ticket.PassengerId = passenger.Id;
                _un.tickets.Add(ticket);
                _un.Save();
                var seat = new Seat
                {
                    PassengerId = passenger.Id,
                    SeatChar = (SeatChar)obj.CharIndex,
                    SeatStatus = SeatStatus.Booked,
                    BusTripId = busTrip.Id,

                };
                _un.seats.Add(seat);
                _un.Save();
            }
            else
            {
                var TripGetter = await _un.busTrip.Get(x => x.Id == obj.tripId,includeProperties: "Seats");
                if (TripGetter != null)
                {
                    SeatChar seatChar = (SeatChar)obj.CharIndex;
                    var seatCheck = TripGetter.Seats.Select(s => s.SeatChar );
                    if (seatCheck.Contains(seatChar)) {
                        return BadRequest(new { message = "the Seat Is Booked", code = BadRequest().StatusCode });
                    }
                    var passenger = new Passenger
                    {
                        ApplicationUserId = User.GetUserId(),
                        ArrivalTime = TripGetter.ArrivalTime,
                        TripType = TripGetter.TripType,
                        BusTripId = TripGetter.Id,
                    };
                    _un.passenger.Add(passenger);
                    _un.Save();
                    ticket.PassengerId = passenger.Id;
                    _un.tickets.Add(ticket);
                    _un.Save();
                    var seat = new Seat
                    {
                        PassengerId = passenger.Id,
                        SeatChar = (SeatChar)obj.CharIndex,
                        SeatStatus = SeatStatus.Booked,
                        BusTripId = TripGetter.Id,

                    };
                    _un.seats.Add(seat);
                    _un.Save();
                }
            }
            

            return Ok(new {message = new{ticketNumber = ticket.Ticketguid },code = Ok().StatusCode});
        }
        [HttpGet("getUserTrips")]
        public async Task<IActionResult> UserTrips()
        {
            var userId = User.GetUserId();
            var tripsIds = _un.passenger.GetAll(x => x.ApplicationUserId == userId).Select(x => x.BusTripId);
            var allTrips = _un.busTrip.GetAll(x => tripsIds.Contains(x.Id), includeProperties: "Passengers,DriverProfile.ApplicationUser")
                .Select(x => new
                {
                    passCount = x.Passengers.Count(),
                    driverData = x.DriverProfile != null && x.DriverProfile.ApplicationUser != null ?
                    new
                    {
                        Name = x.DriverProfile.ApplicationUser.UserName,
                        Phone = x.DriverProfile.ApplicationUser.PhoneNumber,
                        Gender = x.DriverProfile.ApplicationUser.Gender,
                    }:null,
                    tripStatus = x.TripStatus,
                    fromGovernerate = x.FromGovernerate,
                    toGovernerate = x.ToGovernerate,
                    Salary = x.Salary,
                    startDate = x.StartDate,
                    endDate = x.endDate,
                    tripType = x.TripType,
                });
            return Ok(allTrips);
        }
        
    }
}