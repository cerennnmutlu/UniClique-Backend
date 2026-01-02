using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using UniCliqueBackend.Application.DTOs.Common;
using UniCliqueBackend.Application.DTOs.Auth;
using UniCliqueBackend.Application.Interfaces.Services;


namespace UniCliqueBackendAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            await _authService.RegisterAsync(request);
            return Ok(new ApiMessageDto { Message = "Registration successful" });
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(result);
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var userId = Guid.Parse(User.FindFirst("sub")!.Value);
            await _authService.LogoutAsync(userId, request.RefreshToken);
            return Ok(new ApiMessageDto { Message = "Logout successful" });
        }

        [HttpPost("external-login")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalLoginRequestDto request)
        {
            var result = await _authService.ExternalLoginAsync(request);
            return Ok(result);
        }

        [HttpPost("verify-email")]
        [ProducesResponseType(typeof(TokenResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto request)
        {
            var result = await _authService.VerifyEmailAsync(request);
            return Ok(result);
        }
        [HttpPost("reset-db-test")]
        [ProducesResponseType(typeof(ApiMessageDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> ResetDb()
        {
            await _authService.ResetDatabaseAsync();
            return Ok(new ApiMessageDto { Message = "Database reset successfully (Users truncated)." });
        }
    }
}
