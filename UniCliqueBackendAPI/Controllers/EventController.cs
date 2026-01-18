using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCliqueBackend.Application.DTOs.Event;
using UniCliqueBackend.Application.Interfaces.Services;

namespace UniCliqueBackendAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var events = await _eventService.GetAllEventsAsync(userId);
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var evt = await _eventService.GetEventByIdAsync(id, userId);
            if (evt == null) return NotFound("Event not found.");

            return Ok(evt);
        }

        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var events = await _eventService.GetMyEventsAsync(userId);
            return Ok(events);
        }

        [HttpGet("joined")]
        public async Task<IActionResult> GetJoinedEvents()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var events = await _eventService.GetJoinedEventsAsync(userId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var createdEvent = await _eventService.CreateEventAsync(model, userId);
            if (createdEvent == null) return BadRequest("Failed to create event.");

            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] CreateEventDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _eventService.UpdateEventAsync(id, model, userId);
            if (!result) return BadRequest("Failed to update event or unauthorized.");

            return Ok("Event updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _eventService.CancelEventAsync(id, userId);
            if (!result) return BadRequest("Failed to cancel event or unauthorized.");

            return Ok("Event cancelled successfully.");
        }

        [HttpPost("{id}/join")]
        public async Task<IActionResult> JoinEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _eventService.JoinEventAsync(id, userId);
            if (!result) return BadRequest("Failed to join event (Full, Ended, or Already Joined).");

            return Ok("Joined event successfully.");
        }

        [HttpPost("{id}/leave")]
        public async Task<IActionResult> LeaveEvent(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _eventService.LeaveEventAsync(id, userId);
            if (!result) return BadRequest("Failed to leave event.");

            return Ok("Left event successfully.");
        }
    }
}
