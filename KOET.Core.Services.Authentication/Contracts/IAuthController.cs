using KOET.Core.Services.Authentication.Messages;
using Microsoft.AspNetCore.Mvc;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IAuthController
    {
        Task<IActionResult> ChangePassword(ChangePasswordRequest request);
        Task<IActionResult> Confirm(string token);
        Task<IActionResult> Login(LoginRequest request);
        Task<IActionResult> Logout();
        Task<IActionResult> Refresh(RefreshTokenRequest request);
        Task<IActionResult> Register(RegisterRequest request);
        Task<IActionResult> RequestReset(ResetPasswordRequest request);
        Task<IActionResult> ResetPassword(SetNewPasswordRequest request);
        Task<IActionResult> UpdateProfile([FromForm] UpdateProfileRequest request);
    }
}