using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Neo4jClient;

namespace KOET.Core.Services.Authentication.Services
{
    public class SessionService : ISessionService
    {
        private readonly IGraphClient _client;

        public SessionService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<string> StartSessionAsync(string userId, string ip, string device)
        {
            var session = new UserSession
            {
                UserId = userId,
                IpAddress = ip,
                DeviceInfo = device
            };

            await _client.Cypher
                .Create("(s:UserSession $session)")
                .WithParam("session", session)
                .ExecuteWithoutResultsAsync();

            return session.Id;
        }

        public async Task EndSessionAsync(string sessionId)
        {
            await _client.Cypher
                .Match("(s:UserSession)")
                .Where((UserSession s) => s.Id == sessionId)
                .Set("s.LogoutTime = datetime()")
                .ExecuteWithoutResultsAsync();
        }

        public async Task<List<UserSession>> GetUserSessionsAsync(string userId)
        {
            return (await _client.Cypher
                .Match("(s:UserSession)")
                .Where((UserSession s) => s.UserId == userId)
                .Return(s => s.As<UserSession>())
                .ResultsAsync).ToList();
        }
    }
}
