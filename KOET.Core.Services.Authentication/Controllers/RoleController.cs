using KOET.Core.Services.Authentication.Contracts;
using KOET.Core.Services.Authentication.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KOET.Core.Services.Authentication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _service;

        public RoleController(IRoleService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> Create(CreateRoleRequest request)
            => Ok(await _service.CreateRoleAsync(request.Name));

        [Authorize(Roles = "Admin")]
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(CreateRoleRequest request)
            => Ok(await _service.DeleteRoleAsync(request.Name));

        [Authorize(Roles = "Admin")]
        [HttpPost("assign")]
        public async Task<IActionResult> Assign(AssignRoleRequest request)
            => Ok(await _service.AssignRoleToUserAsync(request.UserId, request.RoleName));

        [Authorize(Roles = "Admin")]
        [HttpPost("remove")]
        public async Task<IActionResult> Remove(AssignRoleRequest request)
            => Ok(await _service.RemoveRoleFromUserAsync(request.UserId, request.RoleName));

        [Authorize(Roles = "Admin")]
        [HttpPost("add-permission")]
        public async Task<IActionResult> AddPermission(PermissionRequest request)
            => Ok(await _service.AddPermissionToRoleAsync(request.RoleName, request.PermissionName));

        [Authorize(Roles = "Admin")]
        [HttpPost("link-role")]
        public async Task<IActionResult> LinkRole(RelatedRoleRequest request)
            => Ok(await _service.LinkRolesAsync(request.SourceRole, request.TargetRole));
    }
}
