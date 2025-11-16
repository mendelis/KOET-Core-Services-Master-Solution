using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Messages;
using KOET.Core.Services.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KOET.Core.Services.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;

        public AuthController(IUserService service)
        {
            _service = service;
        }

        private string GetClientIp() =>
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        private string GetDeviceInfo() =>
            Request.Headers["User-Agent"].ToString();

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
            => Ok(await _service.RegisterAsync(request));

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
            => Ok(await _service.LoginAsync(request, GetClientIp(), GetDeviceInfo()));

        [HttpGet("confirm")]
        public async Task<IActionResult> Confirm(string token)
            => Ok(await _service.ConfirmEmailAsync(token) ? "Confirmed" : "Invalid token");

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh(RefreshTokenRequest request)
            => Ok(await _service.RefreshTokenAsync(request));

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var sessionId = User.FindFirst("session_id")?.Value;
            if (sessionId == null) return BadRequest("Session ID missing.");
            await _service.LogoutAsync(sessionId);
            return Ok("Logged out.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var success = await _service.ChangePasswordAsync(userId, request);
            return success ? Ok("Password changed.") : BadRequest("Invalid current password.");
        }

        [HttpPost("request-reset")]
        public async Task<IActionResult> RequestReset(ResetPasswordRequest request)
        {
            var success = await _service.RequestPasswordResetAsync(request.Email);
            return success ? Ok("Reset link sent.") : NotFound("User not found.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(SetNewPasswordRequest request)
        {
            var success = await _service.SetNewPasswordAsync(request);
            return success ? Ok("Password updated.") : BadRequest("Invalid or expired token.");
        }
    }
}
