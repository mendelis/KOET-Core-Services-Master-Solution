using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Models;
using Neo4jClient;

namespace KOET.Core.Services.Authentication.Services
{
    public class RoleService : IRoleService
    {
        private readonly IGraphClient _client;

        public RoleService(IGraphClient client)
        {
            _client = client;
        }

        public async Task<bool> CreateRoleAsync(string name)
        {
            await _client.Cypher
                .Merge("(r:Role {Name: $name})")
                .WithParam("name", name)
                .ExecuteWithoutResultsAsync();
            return true;
        }

        public async Task<bool> DeleteRoleAsync(string name)
        {
            await _client.Cypher
                .Match("(r:Role {Name: $name})")
                .DetachDelete("r")
                .WithParam("name", name)
                .ExecuteWithoutResultsAsync();
            return true;
        }

        public async Task<bool> AssignRoleToUserAsync(string userId, string roleName)
        {
            await _client.Cypher
                .Match("(u:User)", "(r:Role)")
                .Where((User u) => u.Id == userId)
                .AndWhere((Role r) => r.Name == roleName)
                .Merge("(u)-[:HAS_ROLE]->(r)")
                .ExecuteWithoutResultsAsync();
            return true;
        }

        public async Task<bool> RemoveRoleFromUserAsync(string userId, string roleName)
        {
            await _client.Cypher
                .Match("(u:User)-[rel:HAS_ROLE]->(r:Role)")
                .Where((User u) => u.Id == userId)
                .AndWhere((Role r) => r.Name == roleName)
                .Delete("rel")
                .ExecuteWithoutResultsAsync();
            return true;
        }

        public async Task<bool> AddPermissionToRoleAsync(string roleName, string permissionName)
        {
            await _client.Cypher
                .Merge("(p:Permission {Name: $permission})")
                .WithParam("permission", permissionName)
                .Match("(r:Role {Name: $role})")
                .WithParam("role", roleName)
                .Merge("(r)-[:HAS_PERMISSION]->(p)")
                .ExecuteWithoutResultsAsync();
            return true;
        }

        public async Task<bool> LinkRolesAsync(string sourceRole, string targetRole)
        {
            await _client.Cypher
                .Match("(r1:Role {Name: $source})", "(r2:Role {Name: $target})")
                .WithParams(new { source = sourceRole, target = targetRole })
                .Merge("(r1)-[:RELATED_TO]->(r2)")
                .ExecuteWithoutResultsAsync();
            return true;
        }
    }
}
