using KOET.Core.Services.Authentication.Models;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface ISessionService
    {
        Task<string> StartSessionAsync(string userId, string ip, string device);
        Task EndSessionAsync(string sessionId);
        Task<List<UserSession>> GetUserSessionsAsync(string userId);
    }
}
