using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Neo4jClient;

namespace KOET.Core.Services.Authentication.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IGraphClient _client;

        public UserRepository(IGraphClient client)
        {
            _client = client;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return (await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.Email == email)
                .Return(u => u.As<User>())
                .ResultsAsync).FirstOrDefault();
        }

        public async Task<User> GetByTokenAsync(string token)
        {
            return (await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.ConfirmationToken == token)
                .Return(u => u.As<User>())
                .ResultsAsync).FirstOrDefault();
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return (await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.Id == id)
                .Return(u => u.As<User>())
                .ResultsAsync).FirstOrDefault();
        }

        public async Task CreateAsync(User user)
        {
            await _client.Cypher
                .Create("(u:User $user)")
                .WithParam("user", user)
                .ExecuteWithoutResultsAsync();
        }

        public async Task UpdateAsync(User user)
        {
            await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.Id == user.Id)
                .Set("u = $user")
                .WithParam("user", user)
                .ExecuteWithoutResultsAsync();
        }

        public async Task RevokeTokenAsync(string userId, string token)
        {
            await _client.Cypher
                .Match("(u:User)")
                .Where((User u) => u.Id == userId)
                .Set("u.RevokedTokens = coalesce(u.RevokedTokens, []) + $token")
                .WithParam("token", token)
                .ExecuteWithoutResultsAsync();
        }

        public async Task<bool> IsTokenRevokedAsync(string userId, string token)
        {
            var user = await GetByIdAsync(userId);
            return user?.RevokedTokens?.Contains(token) ?? false;
        }
    }
}
