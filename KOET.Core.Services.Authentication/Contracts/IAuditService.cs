namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IAuditService
    {
        Task LogAsync(string userId, string action, string metadata = null);
    }

}
