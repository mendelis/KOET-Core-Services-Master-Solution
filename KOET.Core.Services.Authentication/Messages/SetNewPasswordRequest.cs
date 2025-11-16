namespace KOET.Core.Services.Authentication.Messages
{
    public class SetNewPasswordRequest
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
