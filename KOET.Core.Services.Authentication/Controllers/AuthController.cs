using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Messages;
using KOET.Core.Services.Authentication.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }
}
