using KOET.Core.Services.Authentication.Messages;
using Microsoft.AspNetCore.Mvc;

namespace KOET.Core.Services.Authentication.Contracts
{
    public interface IRoleController
    {
        Task<IActionResult> AddPermission(PermissionRequest request);
        Task<IActionResult> Assign(AssignRoleRequest request);
        Task<IActionResult> Create(CreateRoleRequest request);
        Task<IActionResult> Delete(CreateRoleRequest request);
        Task<IActionResult> LinkRole(RelatedRoleRequest request);
        Task<IActionResult> Remove(AssignRoleRequest request);
    }
}