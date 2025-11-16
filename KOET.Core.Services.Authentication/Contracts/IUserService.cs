using KOET.Core.Services.Authentication.Messages;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request, string ip, string device);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task<bool> ConfirmEmailAsync(string token);
        Task<bool> LogoutAsync(string sessionId);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> SetNewPasswordAsync(SetNewPasswordRequest request);

    }
}
