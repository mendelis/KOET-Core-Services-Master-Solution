using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Neo4jClient;

namespace KOET.Core.Services.Authentication.Services
{
    public class AuditService : IAuditService
    {
        private readonly IGraphClient _client;

        public AuditService(IGraphClient client)
        {
            _client = client;
        }

        public async Task LogAsync(string userId, string action, string metadata = null)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Metadata = metadata
            };

            await _client.Cypher
                .Create("(log:AuditLog $log)")
                .WithParam("log", log)
                .ExecuteWithoutResultsAsync();
        }
    }
}
