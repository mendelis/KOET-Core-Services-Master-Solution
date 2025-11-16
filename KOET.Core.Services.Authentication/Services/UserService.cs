using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Messages;
using KOET.Core.Services.Authentication.Models;
using System.Security.Claims;

namespace KOET.Core.Services.Authentication.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IEmailSender _emailSender;
        private readonly ITokenService _tokenService;
        private readonly ISessionService _sessionService;
        private readonly IAuditService _audit;

        public UserService(IUserRepository repo, IEmailSender emailSender, ITokenService tokenService, ISessionService sessionService, IAuditService audit)
        {
            _repository = repo;
            _emailSender = emailSender;
            _tokenService = tokenService;
            _sessionService = sessionService;
            _audit = audit;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            var existing = await _repository.GetByEmailAsync(request.Email);
            if (existing != null)
                return new AuthResponse { Message = "Email already registered." };

            var user = new User
            {
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                ConfirmationToken = Guid.NewGuid().ToString()
            };

            await _repository.CreateAsync(user);
            await _emailSender.SendConfirmationEmail(user.Email, user.ConfirmationToken);
            await _audit.LogAsync(user.Id, "Register");

            return new AuthResponse { Message = "Registration successful. Please confirm your email." };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request, string ip, string device)
        {
            var user = await _repository.GetByEmailAsync(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return new AuthResponse { Message = "Invalid credentials." };

            if (!user.IsEmailConfirmed)
                return new AuthResponse { Message = "Email not confirmed." };

            var token = _tokenService.GenerateToken(user);
            var refresh = _tokenService.GenerateRefreshToken();
            var sessionId = await _sessionService.StartSessionAsync(user.Id, ip, device);

            user.RefreshToken = refresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "Login", $"Session: {sessionId}");

            return new AuthResponse { Token = token, RefreshToken = refresh, Message = "Login successful." };
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.Token);
            var email = principal.FindFirstValue(ClaimTypes.Email);
            var user = await _repository.GetByEmailAsync(email);

            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
                return new AuthResponse { Message = "Invalid refresh token." };

            var newJwt = _tokenService.GenerateToken(user);
            var newRefresh = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefresh;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "RefreshToken");

            return new AuthResponse { Token = newJwt, RefreshToken = newRefresh, Message = "Token refreshed." };
        }

        public async Task<bool> ConfirmEmailAsync(string token)
        {
            var user = await _repository.GetByTokenAsync(token);
            if (user == null) return false;

            user.IsEmailConfirmed = true;
            user.ConfirmationToken = null;
            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "ConfirmEmail");
            return true;
        }

        public async Task<bool> LogoutAsync(string sessionId)
        {
            await _sessionService.EndSessionAsync(sessionId);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordRequest request)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "ChangePassword");
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _repository.GetByEmailAsync(email);
            if (user == null) return false;

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _repository.UpdateAsync(user);

            var resetLink = $"https://yourdomain.com/reset-password?token={user.ResetToken}";
            var body = $"Click to reset your password: {resetLink}";
            await _emailSender.SendConfirmationEmail(user.Email, user.ResetToken); // reuse method or create new one
            await _audit.LogAsync(user.Id, "RequestPasswordReset");
            return true;
        }

        public async Task<bool> SetNewPasswordAsync(SetNewPasswordRequest request)
        {
            var user = await _repository.GetByResetTokenAsync(request.Token);
            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "SetNewPassword");
            return true;
        }

        public async Task<bool> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await _repository.GetByIdAsync(userId);
            if (user == null) return false;

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.BirthDate = request.BirthDate;

            if (request.Photo != null)
            {
                var fileName = $"PROFILE_{userId}{Path.GetExtension(request.Photo.FileName)}";
                var filePath = Path.Combine("wwwroot", "uploads", fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await request.Photo.CopyToAsync(stream);

                user.PhotoUrl = $"/uploads/{fileName}";
            }

            await _repository.UpdateAsync(user);
            await _audit.LogAsync(user.Id, "UpdateProfile");
            return true;
        }
    }
}
