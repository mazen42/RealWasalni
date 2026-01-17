using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Threading.Tasks;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_Models.DTOs;
using Wasalni_Utility;

namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = SD.Role_Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IUnitOfWork _un;
        public AdminController(IUnitOfWork un)
        {
            _un = un;
        }

        [HttpGet("allDrivers")]
        public IActionResult GetAllDriver()
        {
            var allDriver = _un.driverProfile.GetAll(includeProperties: "ApplicationUser,Bus").Select(s => new { 
                id = s.Id,
                Name = s.ApplicationUser.UserName,
                Email = s.ApplicationUser.Email,
                Phone = s.ApplicationUser.PhoneNumber,
                Age = s.ApplicationUser.Age,
                Status = s.ApprovalStatus,
                veichleType = s.Bus.VehicleType.ToString()

            });
            if (allDriver == null)
                return NotFound(new { message = "Empty", code = NotFound().StatusCode });
            return Ok(allDriver);
        }
        [HttpGet("GetDriver")]
        public async Task<IActionResult> GetDriver(int id)
        {
            var driver = await _un.driverProfile.Get(d => d.Id == id,includeProperties: "ApplicationUser");
            if (driver == null)
                return NotFound(new {message = "no driver found",code = NotFound().StatusCode });

            return Ok(new
            {
                id = driver.Id,
                name = driver.ApplicationUser.UserName
            });
        }
        [HttpPost("UpdateDriverStatus")]
        public async Task<IActionResult> UpdateDriverStatusAsync(UpdateDriverStatusAndDataDTO obj)
        {
            var driver = await _un.driverProfile.Get(d => d.Id == obj.id, tracked: true);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (driver == null)
                return NotFound(new { message = "no driver found", code = NotFound().StatusCode });
            driver.ApprovalStatus = obj.DriverApprovalStatus;
            driver.SubscriptionExpiryDate = DateTime.Now.AddMonths(obj.monthsToWork);
            _un.driverProfile.Update(driver);
            _un.Save();
            return Ok(new { message = "updated", code = Ok().StatusCode});
        }
        
    }
}
