using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wasalni.Services;
using Wasalni_Models;

namespace Wasalni.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationController : ControllerBase
    {
        private readonly BackGroundJobService _jobService;
        private readonly ILogger<InvitationController> _logger;

        public InvitationController(BackGroundJobService jobService, ILogger<InvitationController> logger)
        {
            _jobService = jobService;
            _logger = logger;
        }

        [HttpPost("accept/{invitationId}")]
        public async Task<IActionResult> AcceptInvitation(int invitationId)
        {
            if (invitationId <= 0)
                return BadRequest(new { message = "Invalid invitation ID", code = 400 });

            try
            {
                await _jobService.AcceptInvitationAsync(invitationId);
                return Ok(new { message = "Invitation accepted successfully", code = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting invitation");
                return StatusCode(500, new { message = "Error accepting invitation", code = 500 });
            }
        }

        [HttpPost("reject/{invitationId}")]
        public async Task<IActionResult> RejectInvitation(int invitationId)
        {
            if (invitationId <= 0)
                return BadRequest(new { message = "Invalid invitation ID", code = 400 });

            try
            {
                await _jobService.RejectInvitationAsync(invitationId);
                return Ok(new { message = "Invitation rejected successfully", code = 200 });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting invitation");
                return StatusCode(500, new { message = "Error rejecting invitation", code = 500 });
            }
        }
    }
}