namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IEmailSender
    {
        Task SendConfirmationEmail(string to, string token);
    }
}
