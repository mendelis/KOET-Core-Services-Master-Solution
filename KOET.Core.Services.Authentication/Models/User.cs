namespace KOET.Core.Services.Authentication.Models
{
    public class User
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string ConfirmationToken { get; set; }
        public List<string> Roles { get; set; } = new() { "User" };
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public List<string> RevokedTokens { get; set; } = new();
    }
}
