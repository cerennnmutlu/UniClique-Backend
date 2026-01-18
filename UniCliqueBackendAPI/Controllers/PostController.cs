using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UniCliqueBackend.Application.DTOs.Post;
using UniCliqueBackend.Application.Interfaces.Services;

namespace UniCliqueBackendAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("events/{eventId}/posts")]
        public async Task<IActionResult> CreatePost(Guid eventId, [FromBody] CreatePostDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (model.EventId == Guid.Empty) model.EventId = eventId;
            if (model.EventId != eventId) return BadRequest("Event ID mismatch.");

            var post = await _postService.CreatePostAsync(model, userId);
            if (post == null) return BadRequest("Failed to create post. User must be participant or owner.");

            return CreatedAtAction(nameof(GetPostsByEvent), new { eventId = eventId }, post);
        }

        [HttpDelete("posts/{id}")]
        public async Task<IActionResult> DeletePost(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _postService.DeletePostAsync(id, userId);
            if (!result) return BadRequest("Failed to delete post or unauthorized.");

            return Ok("Post deleted successfully.");
        }

        [HttpGet("events/{eventId}/posts")]
        public async Task<IActionResult> GetPostsByEvent(Guid eventId)
        {
            var posts = await _postService.GetPostsByEventIdAsync(eventId);
            return Ok(posts);
        }

        [HttpGet("users/me/posts")]
        public async Task<IActionResult> GetMyPosts()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var posts = await _postService.GetMyPostsAsync(userId);
            return Ok(posts);
        }

        [HttpGet("users/{userId}/posts")]
        public async Task<IActionResult> GetUserPosts(string userId)
        {
            var posts = await _postService.GetUserPostsAsync(userId);
            return Ok(posts);
        }

        [HttpGet("feed")]
        public async Task<IActionResult> GetFeed()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var feed = await _postService.GetFeedAsync(userId);
            return Ok(feed);
        }
    }
}
