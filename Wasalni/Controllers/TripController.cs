using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Threading.Tasks;
using Wasalni.Infrastructure.Interfaces;
using Wasalni.Services;
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
        private readonly INotificationService notificationService;
        private readonly IBackGroundJobsServices jobs;

        public TripController(IUnitOfWork unitOfWork, HttpClient httpClient, INotificationService notificationService,IBackGroundJobsServices jobs)
        {
            _un = unitOfWork;
            this._httpClient = httpClient;
            this.notificationService = notificationService;
            this.jobs = jobs;
        }
        [HttpPost("UserTripRequestSingle")]
        public async Task<IActionResult> UserTripRequestAsync(TripRequestDTO obj)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var responseFrom = await BuiltInMethods.GetCityFromNominatimAsync(obj.FromLocation.Latitude, obj.FromLocation.Longitude, _httpClient);
            var responseTo = await BuiltInMethods.GetCityFromNominatimAsync(obj.ToLocation.Latitude, obj.ToLocation.Longitude, _httpClient);

            var PassengerRepeatingCheck = await _un.passenger.Get(x => x.FromLocation!.Longitude == obj.FromLocation.Longitude && x.FromLocation.Latitude == obj.FromLocation.Latitude && x.ToLocation.Longitude == obj.ToLocation.Longitude && x.ToLocation.Latitude == obj.ToLocation.Latitude && x.ApplicationUserId == User.GetUserId() && x.ArrivalTime == obj.arrivalTime);
            if (PassengerRepeatingCheck is not null)
                return BadRequest(new { message = "Trip Already Requested", code = BadRequest().StatusCode });
            if (responseFrom is null || responseTo is null)
                return BadRequest(new { code = BadRequest().StatusCode, message = "Invalid Location" });
            var friendsEmails = _un.applicationUser.GetAll(x => obj.FriendsEmails.Contains(x.Email!)).Select(z => z.Email).ToList();
            if (friendsEmails.Count() != obj.FriendsEmails.Count())
                return BadRequest(new { message = "no emails found", code = BadRequest().StatusCode });

            var ExsitsTripsCheck = _un.busTrip.GetAll(
    r => r.FromGovernerate == responseFrom &&
         r.ToGovernerate == responseTo &&
         r.ArrivalTime == obj.arrivalTime &&
         r.VehicleType == obj.VehicleType &&
         r.TripType == obj.TripType &&
         r.Passengers.Count() < friendsEmails.Count() &&
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
                return Ok(new { message = seatsDict, freindsEmails = friendsEmails, tripId = ExsitsTripsCheck.Id, tripData = objToSend, code = Ok().StatusCode });
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

                return Ok(new { message = dict, freindsEmails = friendsEmails, tripId = 0, tripData = objToSend, code = Ok().StatusCode });
            }
        }
        [HttpPost("BookTheTripWithSeatChar")]
        public async Task<IActionResult> BookTheTripWithSeatChar(TripBookDTO obj)
        {
            var userId = User.GetUserId();
            var usergetter = await _un.applicationUser.Get(x => x.Id == userId);
            var userSeat = (SeatChar)obj.FriendsEmailsWithSeats[usergetter.Email!];
            var TripId = 0;

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
                TripId = busTrip.Id;
                var passenger = new Passenger
                {
                    ApplicationUserId = userId,
                    ArrivalTime = obj.arrivalTime,
                    DaysLeft = 30,
                    TripType = obj.TripType,
                    BusTripId = busTrip.Id,
                };
                _un.passenger.Add(passenger);
                _un.Save();
                var seat = new Seat
                {
                    PassengerId = passenger.Id,
                    SeatChar = (SeatChar)obj.FriendsEmailsWithSeats[usergetter.Email!],
                    SeatStatus = SeatStatus.Booked,
                    BusTripId = busTrip.Id,

                };
                _un.seats.Add(seat);
                _un.Save();
            }
            else
            {
                var TripGetter = await _un.busTrip.Get(x => x.Id == obj.tripId, includeProperties: "Seats");
                if (TripGetter != null)
                {
                    TripId = TripGetter.Id;
                    SeatChar seatChar = userSeat;
                    var seatCheck = TripGetter.Seats.Select(s => s.SeatChar);
                    if (seatCheck.Contains(seatChar)) {
                        return BadRequest(new { message = "the Seat Is Booked", code = BadRequest().StatusCode });
                    }
                    var passenger = new Passenger
                    {
                        ApplicationUserId = userId,
                        ArrivalTime = TripGetter.ArrivalTime,
                        TripType = TripGetter.TripType,
                        BusTripId = TripGetter.Id,
                    };
                    _un.passenger.Add(passenger);
                    _un.Save();
                    var seat = new Seat
                    {
                        PassengerId = passenger.Id,
                        SeatChar = userSeat,
                        SeatStatus = SeatStatus.Booked,
                        BusTripId = TripGetter.Id,

                    };
                    _un.seats.Add(seat);
                    _un.Save();
                }
            }
            var users = _un.applicationUser.GetAll(x => obj.FriendsEmailsWithSeats.Keys.Contains(x.Email!) && x.Email != usergetter.Email);
            foreach (var user in users)
            {
                var virtualSeat = new Seat
                {
                    SeatChar = (SeatChar)obj.FriendsEmailsWithSeats[user.Email],
                    SeatStatus = SeatStatus.pending,
                    BusTripId= TripId,

                };
                _un.seats.Add(virtualSeat);
                _un.Save();
                var invite = new Invitation
                {
                    SenderId = userId,
                    ReceiverId = user.Id,
                    ExpiresAt = TimeOnly.FromDateTime(DateTime.Now.AddMinutes(1)),
                    Status = InvitationStatus.Pending,
                    BusTripId = TripId,
                    seatChar = virtualSeat.SeatChar
                };
                _un.invitation.Add(invite);
                _un.Save();
                await notificationService.sendInvite(user.Email, TripId, (SeatChar)obj.FriendsEmailsWithSeats[user.Email], invite.Id);
            }

            return Ok(new { message = "Booked", code = Ok().StatusCode });
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
                    tripId = x.Id,
                    driverData = x.DriverProfile != null && x.DriverProfile.ApplicationUser != null ?
                    new
                    {
                        Name = x.DriverProfile.ApplicationUser.UserName,
                        Phone = x.DriverProfile.ApplicationUser.PhoneNumber,
                        Gender = x.DriverProfile.ApplicationUser.Gender,
                    } : null,
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
        [HttpGet("GetTripDetails")]
        public async Task<IActionResult> GetTripDetails(int tripId)
        {
            var usergetter = await _un.applicationUser.Get(x => x.Id == User.GetUserId());
            var trip = await _un.busTrip.Get(x => x.Id == tripId, includeProperties: "Passengers,DriverProfile.ApplicationUser");
            if (trip is not null)
            {
                var obj = new
                {
                    fromGovernerate = trip.FromGovernerate,
                    toGovernerate = trip.ToGovernerate,
                    arrivalTime = trip.ArrivalTime,
                    vehicleType = trip.VehicleType,
                    tripType = trip.TripType,
                };
                return Ok(new { message = obj, code = Ok().StatusCode });
            }
            else
            {
                return BadRequest();
            }

        }

    } 
}