using KOET.Core.Services.Authentication.Models;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IRoleService
    {
        Task<bool> CreateRoleAsync(string name);
        Task<bool> DeleteRoleAsync(string name);
        Task<bool> AssignRoleToUserAsync(string userId, string roleName);
        Task<bool> RemoveRoleFromUserAsync(string userId, string roleName);
        Task<bool> AddPermissionToRoleAsync(string roleName, string permissionName);
        Task<bool> LinkRolesAsync(string sourceRole, string targetRole);
        Task<IEnumerable<Role>> GetAllRolesAsync();

    }
}
