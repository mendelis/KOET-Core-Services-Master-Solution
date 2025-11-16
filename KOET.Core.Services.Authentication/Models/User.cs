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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? PhotoUrl { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
