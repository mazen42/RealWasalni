using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Wasalni.Infrastructure.Interfaces;
using Wasalni_Models;
using Wasalni_Models.DTOs;
using Wasalni_Utility;

namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountUserController : ControllerBase
    {
        public IUnitOfWork _unitOfWork { get; }
        public UserManager<ApplicationUser> _userManager { get; }
        public IConfiguration Confg { get; }
        public IWebHostEnvironment _webHostEnvironment { get; }

        public AccountUserController(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager,IConfiguration Confg, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            this.Confg = Confg;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromForm] RegisterUserDTO obj)
        {
            ApplicationUser emailCheck = await _userManager.FindByEmailAsync(obj.Email);
            if (emailCheck != null)
            {
                return BadRequest(new {message= "Email already exsits." ,code = BadRequest().StatusCode});
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            ApplicationUser user = new ApplicationUser
            {
                Email = obj.Email,
                PhoneNumber = obj.PhoneNumber,
                UserName = obj.Name,
                Gender = obj.Gender,
                UserType = obj.UserType,
                Age = obj.Age,
            };
            if (obj.HomeLocation != null) {
                user.HomeLocation = new Location
                {
                    Latitude = obj.HomeLocation.Latitude,
                    Longitude = obj.HomeLocation.Longitude
                };
            }
            IdentityResult create = await _userManager.CreateAsync(user, obj.Password);
                if (!create.Succeeded)
                {
                    foreach (var error in create.Errors)
                    {
                    if (error.Code.Contains("Username"))
                    {
                        ModelState.AddModelError("username", error.Description);
                    }
                    else
                    {
                        ModelState.AddModelError("",error.Description);
                    }
                    }
                    return BadRequest(ModelState);
                }
            if (obj.UserType == UserType.Driver)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                string idCardsPath = Path.Combine(wwwRootPath, @"images\IdCards");
                Directory.CreateDirectory(idCardsPath);

                string idCardFaceName = Guid.NewGuid().ToString() + Path.GetExtension(obj.IdCardFace!.FileName);
                string idCardBackName = Guid.NewGuid().ToString() + Path.GetExtension(obj.IdCardBack!.FileName);

                using (var faceStream = new FileStream(Path.Combine(idCardsPath, idCardFaceName), FileMode.Create))
                {
                    obj.IdCardFace.CopyTo(faceStream);
                }
                using (var backStream = new FileStream(Path.Combine(idCardsPath, idCardBackName), FileMode.Create))
                {
                    obj.IdCardBack.CopyTo(backStream);
                }

                string licenseCardsPath = Path.Combine(wwwRootPath, @"images\LicenseCards");
                Directory.CreateDirectory(licenseCardsPath);

                string licenseFaceName = Guid.NewGuid().ToString() + Path.GetExtension(obj.LicenseImageFace!.FileName);
                string licenseBackName = Guid.NewGuid().ToString() + Path.GetExtension(obj.LicenseImageBack!.FileName);

                using (var faceStream = new FileStream(Path.Combine(licenseCardsPath, licenseFaceName), FileMode.Create))
                {
                    obj.LicenseImageFace.CopyTo(faceStream);
                }
                using (var backStream = new FileStream(Path.Combine(licenseCardsPath, licenseBackName), FileMode.Create))
                {
                    obj.LicenseImageBack.CopyTo(backStream);
                }
                DriverProfile driverProfile = new DriverProfile
                {
                    IdCardFaceURL = @"\images\IdCards\" + idCardFaceName,
                    IdCardBackURL = @"\images\IdCards\" + idCardBackName,
                    LicenseImageFaceURL = @"\images\LicenseCards\" + licenseFaceName,
                    LicenseImageBackURL = @"\images\LicenseCards\" + licenseBackName,
                    ApprovalStatus = DriverApprovalStatus.Pending,
                    ApplicationUserId = user.Id,
                    SubscriptionExpiryDate = DateTime.Now.AddMonths(obj.monthsOfWork)
                };
                _unitOfWork.driverProfile.Add(driverProfile);
                _unitOfWork.Save();
                Bus bus = new Bus
                {
                    VehicleType = obj.VehicleType,
                    PlateNumber = obj.PlateNumber!,
                    DriverProfileId = driverProfile.Id
                };
                _unitOfWork.bus.Add(bus);
                _unitOfWork.Save();
                await _userManager.AddToRoleAsync(user, SD.Role_Driver);
                return Ok(new {message = "driver created successfully", code = Ok().StatusCode});
            }
            if (create.Succeeded) {
                await _userManager.AddToRoleAsync(user, SD.Role_Passenger);
            }

            return Ok(new
            {
                code = 200,
                message = "User registered successfully"
            });
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO obj)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var usercheck = await _userManager.FindByEmailAsync(obj.Email);

            if (usercheck == null)
                return BadRequest(new {message = "Invalid Email or Password",code = 400 });

            bool passCheck = await _userManager.CheckPasswordAsync(usercheck, obj.Password);
            if (!passCheck)
                return BadRequest(new { message = "Invalid UserName or Password", code = 400 });
            List<Claim> claims = new List<Claim>() {
        new Claim(ClaimTypes.Email, usercheck.Email!),
        new Claim(ClaimTypes.NameIdentifier, usercheck.Id),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim("HomeLocation", usercheck.HomeLocation?.ToString() ?? ""),
    };
            foreach (var role in await _userManager.GetRolesAsync(usercheck))
                claims.Add(new Claim(ClaimTypes.Role, role));

            var expiry = obj.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Confg["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                expires: expiry,
                claims: claims,
                signingCredentials: creds
            );
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiry
            });
        }

    }
}
