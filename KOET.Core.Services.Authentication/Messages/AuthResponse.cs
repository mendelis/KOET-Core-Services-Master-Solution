using KOET.Core.Services.Authentication.Models;

namespace KOET.Core.Services.Authentication.Messages
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string Message { get; set; }
        public BaseUser User { get; set; }
    }
}
