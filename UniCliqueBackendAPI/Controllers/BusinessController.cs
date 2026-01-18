using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCliqueBackend.Application.DTOs.Business;
using UniCliqueBackend.Application.Interfaces.Services;

namespace UniCliqueBackendAPI.Controllers
{
    [ApiController]
    [Route("api/business")]
    [Authorize]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> CreateRequest([FromBody] CreateBusinessRequestDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _businessService.CreateBusinessRequestAsync(userId, model);
            if (!result) return BadRequest("Failed to create request. You might already have a pending request.");

            return Ok("Business account request submitted successfully.");
        }

        [HttpGet("my-request")]
        public async Task<IActionResult> GetMyRequest()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var request = await _businessService.GetMyRequestAsync(userId);
            if (request == null) return NotFound("No request found.");

            return Ok(request);
        }

        [HttpGet("stats")]
        [Authorize(Roles = "Business,Admin")]
        public async Task<IActionResult> GetStats()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var stats = await _businessService.GetBusinessStatsAsync(userId);
            return Ok(stats);
        }

        // Admin Endpoints
        [HttpGet("requests")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingRequests()
        {
            var requests = await _businessService.GetPendingRequestsAsync();
            return Ok(requests);
        }

        [HttpPut("requests/{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveRequest(Guid id)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminId)) return Unauthorized();

            var result = await _businessService.ApproveRequestAsync(id, adminId);
            
            if (!result) return BadRequest("Failed to approve request.");
            return Ok("Request approved. User role updated to Business.");
        }

        [HttpPut("requests/{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RejectRequest(Guid id, [FromQuery] string reason)
        {
            var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(adminId)) return Unauthorized();

            var result = await _businessService.RejectRequestAsync(id, adminId, reason ?? "No reason provided.");
            
            if (!result) return BadRequest("Failed to reject request.");
            return Ok("Request rejected.");
        }
    }
}
