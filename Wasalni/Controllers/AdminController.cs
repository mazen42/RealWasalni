using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Data;
using System.Threading.Tasks;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_Models;
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
        public async Task<IActionResult> GetAllDriverAsync()
        {
            var allDriver =  _un.driverProfile
                .GetAll(includeProperties: "ApplicationUser,Bus")
                .Select(s => new {
                    id = s.Id,
                    Name = s.ApplicationUser.UserName,
                    email = s.ApplicationUser.Email,
                    Phone = s.ApplicationUser.PhoneNumber,
                    age = s.ApplicationUser.Age,
                    Status = s.ApprovalStatus,
                    veichleType = s.Bus.VehicleType
                })
                .ToList();

            if (!allDriver.Any())
                return NotFound(new { message = "Empty", code = NotFound().StatusCode });

            return Ok(allDriver);
        }
        [HttpGet("GetDriver")]
        public async Task<IActionResult> GetDriver(int id)
        {
            var driver = await _un.driverProfile.Get(d => d.Id == id,includeProperties: "ApplicationUser,Bus");
            if (driver == null)
                return NotFound(new {message = "no driver found",code = NotFound().StatusCode });

            return Ok(new
            {
                id = driver.Id,
                name = driver.ApplicationUser.UserName,
                email = driver.ApplicationUser.Email,
                Phone = driver.ApplicationUser.PhoneNumber,
                Age = driver.ApplicationUser.Age,
                idCardFace = driver.IdCardFaceURL,
                idCardBack = driver.IdCardBackURL,
                licenseFace = driver.LicenseImageFaceURL,
                licenseBack = driver.LicenseImageBackURL,
                veichleType = driver.Bus.VehicleType,
                status = driver.ApprovalStatus
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
            _un.driverProfile.Update(driver);
            _un.Save();
            return Ok(new { message = "updated", code = Ok().StatusCode});
        }
        
    }
}
