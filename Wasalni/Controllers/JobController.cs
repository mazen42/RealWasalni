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
        [HttpPost("DecrementPassengersLeftDaysInAllTripsAsyncJobCheck")]
        public async Task<IActionResult> DecrementPassengersLeftDaysInAllTripsAsyncJob()
        {
             await BackGroundJobsServices.DecrementPassengersLeftDaysInAllTripsAsync();
            return Ok("Executed Successfully");
        }
        [HttpPost("RecjectionJob")]
        public async Task<IActionResult> RecjectionJob()
        {
             await BackGroundJobsServices.InvitesRejectionJob();
            return Ok("Executed Successfully");
        }
    }
}
