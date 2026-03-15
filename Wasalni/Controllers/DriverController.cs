using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_DataAccess.Data;
using Wasalni_Models;
using Wasalni_Models.DTOs;
using Wasalni_Utility;
namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]

    public class DriverController : ControllerBase
    {
        public IUnitOfWork _unitOfWork { get; }
        public AppDbContext _db {  get; set; }
        public UserManager<ApplicationUser> _usermanager { get; }
        public IWebHostEnvironment _webHostEnvironment { get; }
        public IConfiguration _configuration { get; }
        public HttpClient _httpClient { get; }

        public DriverController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> usermanager,IWebHostEnvironment webHostEnvironment,IConfiguration configuration,HttpClient httpClient, AppDbContext appDbContext)
        {
            _unitOfWork = unitOfWork;
            _usermanager = usermanager;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
            _httpClient = httpClient;
            _db = appDbContext;
        }
        [HttpPost("RequestTripnominatim")]
        public async Task<IActionResult> RequestTripAsync(DriverTripRequestDTO obj)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            if((obj.StartTime > obj.EndTime))
                return BadRequest(new { message = "Start Time Must Be Less Than End Time", code = BadRequest().StatusCode});
            if ((obj.EndTime - obj.StartTime).TotalHours < 4)
                return BadRequest(new { message = "The Duration Must Be Greater Than 4 Hours", code = BadRequest().StatusCode});
            var responseFrom = await BuiltInMethods.GetCityFromNominatimAsync(obj.FromLocation.Latitude, obj.FromLocation.Longitude, _httpClient);
            var responseTo = await BuiltInMethods.GetCityFromNominatimAsync(obj.ToLocation.Latitude, obj.ToLocation.Longitude, _httpClient);
            var DublicateTripCheck = await _unitOfWork.driverTripRequest.Get(x => x.ToGovernerate == responseTo && x.FromGovernerate == responseFrom && x.RequestStatus == DriverTripRequestStatus.Requested);
            if (DublicateTripCheck is not null)
                return BadRequest(new { message = "Trip Already Requested", code = BadRequest().StatusCode });
            if (responseFrom != null && responseTo != null)
            {
            var userId = User.GetUserId();
            var driverId = User.GetDriverId(_db);

            DriverTripRequest driverTripRequest = new DriverTripRequest
            {
                DriverId = driverId,
                FromGovernerate = responseFrom,
                ToGovernerate = responseTo,
                StartTime = obj.StartTime,
                EndTime = obj.EndTime,
            };
                _unitOfWork.driverTripRequest.Add(driverTripRequest);
                _unitOfWork.Save();
                return NoContent();
            }
                return BadRequest(new { message = "Trip Request Failed",code = BadRequest().StatusCode });
        }
        
        [HttpGet("GetAllTripsRequests")]
        
        public IActionResult GetAllTripsRequests() {
            var driverId = User.GetDriverId(_db);
            var allTripRequests = _unitOfWork.driverTripRequest.GetAll(d => d.DriverId == driverId && d.RequestStatus == DriverTripRequestStatus.Requested).Select(s => new
            {
                id = s.Id,
                fromGovernerate = s.FromGovernerate,
                toGovernerate = s.ToGovernerate,
                startTime = s.StartTime,
                endTime = s.EndTime,
                status = s.RequestStatus
            });
            if (allTripRequests != null)
                return Ok(allTripRequests);
            return NoContent();
        }
        [HttpGet("GetAllTrips")]
        public async Task<IActionResult> GetAcceptedTripsAsync() {
            var driverId = User.GetDriverId(_db);
            var trips = _unitOfWork.busTrip.GetAll(b => b.DriverProfileId == driverId && b.TripStatus != TripStatus.Pending).Select(b => new
            {
                id = b.Id,
                from = b.FromGovernerate,
                to = b.ToGovernerate,
                startdate = (b.StartDate)?.ToString("MM/dd"),
                enddate = (b.endDate)?.ToString("MM/dd"),
                status = b.TripStatus
            });
            
            return Ok(trips);
        }
        [HttpGet("GetAllAppropriateTrips")]
        public async Task<IActionResult> GetAllAppropriateTrips(string fromPlace, string toPlace)
        {
            var driverId = User.GetDriverId(_db);
            var driver = await _unitOfWork.driverProfile.Get(x => x.Id == driverId,includeProperties:"Bus");
            var appropriateTrips = _unitOfWork.busTrip.GetAll(x => x.DriverProfileId == null && x.FromGovernerate == fromPlace && x.ToGovernerate == toPlace && x.VehicleType == driver.Bus.VehicleType,includeProperties:"Passengers").Select(d => new
            {
                id = d.Id,
                from = d.FromGovernerate,
                to = d.ToGovernerate,
                arrivalTime = d.ArrivalTime,
                passenger = d.Passengers.Count(),
                price = d.Price,
            }).ToList();
            return appropriateTrips.Count() <= 0 ? BadRequest(new { code = BadRequest().StatusCode, message = "no trips" }) : Ok(new { code = Ok().StatusCode, message = appropriateTrips });
        }
        [HttpPost("ticketValidation")]
        public async Task<IActionResult> ticketValidation(TicketValidationDTO obj)
        {
            //var trip = _db.BusTrips.Include(x => x.Passengers).ThenInclude(x => x.Ticket).FirstOrDefault(x => x.Id == obj.TripId).Passengers.FirstOrDefault(x => x.Ticket.Ticketguid == obj.TicketNumber);
            var ticket = _db.BusTrips.Include(x => x.Passengers).ThenInclude(x => x.Ticket).FirstOrDefault(x => x.Id == obj.TripId).Passengers.Select(x => x.Ticket).FirstOrDefault(x => x.Ticketguid == obj.TicketNumber);
            if (ticket == null)
            {
                return BadRequest(new { message = "invalid Trip", code = BadRequest().StatusCode });
            }
            if (ticket.QRstatus)
            {
                return BadRequest(new { message = "already checked", code = BadRequest().StatusCode });
            }
            else
            {
                ticket.QRstatus = true;
                _unitOfWork.Save();
                return Ok(new { message = "Verified", code = Ok().StatusCode });
            }
        }
        [HttpPost("getAllPassengerOfTrip")]
        public async Task<IActionResult> getAllPassengerOfTrip(int tripId)
        {
            var trip =  await _unitOfWork.busTrip.Get(x => x.Id == tripId,includeProperties: "Passengers.ApplicationUser,Passengers.Ticket,Passengers.Seat");
            var passengers = trip.Passengers.Select(x => new
            {
                name = x.ApplicationUser.UserName,
                TicketStatus = x.Ticket.QRstatus ? "Verified" : "Pending",
                seatChar = (int)x.Seat.SeatChar,
            });
            return Ok(new { message = passengers, code = Ok().StatusCode });

        }



    }
}
