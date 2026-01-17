using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Wasalni.Services;

namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        public IBackGroundJobsServices BackGroundJobsServices { get; }
        public JobController(IBackGroundJobsServices backGroundJobsServices)
        {
            BackGroundJobsServices = backGroundJobsServices;
        }


        [HttpPost("FillingBusTripsByDriversAndSendNotificationToAllPassengersJobCheck")]
        public async Task<IActionResult> CreateBackGroundJob()
        {
             BackGroundJobsServices.FillingBusTripsByDriversAndSendNotificationToAllPassengers();
            return Ok("Executed Successfully");
        }
        [HttpPost("DecrementPassengersLeftDaysInAllTripsAsyncJobCheck")]
        public async Task<IActionResult> DecrementPassengersLeftDaysInAllTripsAsyncJob()
        {
             await BackGroundJobsServices.DecrementPassengersLeftDaysInAllTripsAsync();
            return Ok("Executed Successfully");
        }
    }
}
