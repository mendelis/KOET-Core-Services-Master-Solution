namespace KOET.Core.Services.Authentication.Models
{
    public class User : BaseUser
    {
        public string PasswordHash { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string ConfirmationToken { get; set; }        
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public List<string> RevokedTokens { get; set; } = new();       
        public DateTime? BirthDate { get; set; }        
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpiry { get; set; }
    }
}
